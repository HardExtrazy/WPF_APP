using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPF_APP.Models;

namespace WPF_APP.ViewModel
{
    public class TeacherVM : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<Teacher> _teachers;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetField(ref _teachers, value);
        }
        public TeacherVM()
        {
            _httpClient = new HttpClient();
            Teachers = new ObservableCollection<Teacher>();

            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        public async Task LoadTeachersAsync(string authToken)
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
                var response = await _httpClient.GetAsync("api/Teacher/teachers");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Десериализуем ответ
                    var teachers = JsonSerializer.Deserialize<Teacher[]>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Очищаем и заполняем коллекцию
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Teachers.Clear();
                        if (teachers != null)
                        {
                            foreach (var teacher in teachers)
                            {
                                Teachers.Add(teacher);
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