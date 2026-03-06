using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPF_APP;
using WPF_APP.Models;
using WPF_APP.View;

namespace WPF_APP.ViewModel
{
    class ClassVM : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<Class> _classes;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetField(ref _classes, value);
        }

        public ClassVM()
        {
            _httpClient = new HttpClient();
            Classes = new ObservableCollection<Class>();

            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        public async Task LoadClassesAsync(string authToken)
        {
            // Очищаем старые заголовки
            _httpClient.DefaultRequestHeaders.Clear();

            // Добавляем токен
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            // Добавляем Accept заголовок
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Отправляем запрос
            var response = await _httpClient.GetAsync("api/Class/classes");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Десериализуем ответ
                var classes = JsonSerializer.Deserialize<Class[]>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Очищаем и заполняем коллекцию
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Classes.Clear();
                    if (classes != null)
                    {
                        foreach (var clas in classes)
                        {
                            Classes.Add(clas);
                        }
                    }
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var cusmomWindow = new CustomMessageBox($"Ошибка сервера: {response.StatusCode}\n{errorContent}");
                cusmomWindow.Show();
            }
        }





        private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
