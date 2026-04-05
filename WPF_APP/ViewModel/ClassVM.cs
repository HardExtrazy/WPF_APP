using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_APP.Models;
using WPF_APP.Utilities;
using System.Linq;

namespace WPF_APP.ViewModel
{
    public class ClassVM : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<Class> _classes;
        private Class _selectedClass;
        private List<Class> _allClasses;
        private string _searchText;
        private ICommand _deleteCommand;
        private ICommand _refreshCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Class> Classes
        {
            get => _classes;
            set => SetField(ref _classes, value);
        }

        public Class SelectedClass
        {
            get => _selectedClass;
            set
            {
                SetField(ref _selectedClass, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        async (param) => await ExecuteDelete(param),
                        (param) => SelectedClass != null
                    );
                }
                return _deleteCommand;
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        async (param) => await LoadClassesAsync(),
                        (param) => true
                    );
                }
                return _refreshCommand;
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                ApplyFilter();  // Применяем фильтр при изменении текста
            }
        }
        public ClassVM()
        {
            _httpClient = new HttpClient();
            Classes = new ObservableCollection<Class>();
            _allClasses = new List<Class>();

            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        public async Task LoadClassesAsync()
        {
            try
            {
                // Добавляем токен авторизации
                if (Application.Current.Properties["AuthToken"] is string token)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync("api/Class/classes");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Используем JsonSerializer для десериализации
                    var classDtos = JsonSerializer.Deserialize<List<Class>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allClasses.Clear();  // Очищаем оригинальные данные

                        if (classDtos != null)
                        {
                            foreach (var dto in classDtos)
                            {
                                _allClasses.Add(new Class
                                {
                                    FullClassName = dto.FullClassName,
                                    Mentor = dto.Mentor,
                                    WorkLoad = dto.WorkLoad,
                                    Shift = dto.Shift
                                });
                            }
                        }

                        ApplyFilter();  // Применяем фильтр (если есть текст поиска)
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера: {response.StatusCode}\n{errorContent}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }

        public async Task DeleteClassAsync(string fullClassName)
        {
            try
            {
                // Добавляем токен авторизации
                if (Application.Current.Properties["AuthToken"] is string token)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                // URL-кодируем название класса
                string encodedClassName = Uri.EscapeDataString(fullClassName);
                var response = await _httpClient.DeleteAsync($"api/Class/{encodedClassName}");

                if (response.IsSuccessStatusCode)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Удаляем из оригинальной коллекции
                        var classToRemove = _allClasses.FirstOrDefault(c => c.FullClassName == fullClassName);
                        if (classToRemove != null)
                        {
                            _allClasses.Remove(classToRemove);
                            ApplyFilter();  // Обновляем отображение
                            SelectedClass = null;
                            MessageBox.Show("Класс успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера: {response.StatusCode}\n{errorContent}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteDelete(object parameter)
        {
            try
            {
                if (SelectedClass == null)
                {
                    MessageBox.Show("Выберите класс для удаления", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить класс {SelectedClass.FullClassName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await DeleteClassAsync(SelectedClass.FullClassName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ApplyFilter()
        {
            Classes.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Показываем все классы
                foreach (var classItem in _allClasses)
                {
                    Classes.Add(classItem);
                }
            }
            else
            {
                // Ищем по названию класса, классному руководителю или смене
                var filtered = _allClasses.Where(classItem =>
                    (classItem.FullClassName != null && classItem.FullClassName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (classItem.Mentor != null && classItem.Mentor.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    classItem.Shift.ToString().IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    classItem.WorkLoad.ToString().IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();

                foreach (var classItem in filtered)
                {
                    Classes.Add(classItem);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}