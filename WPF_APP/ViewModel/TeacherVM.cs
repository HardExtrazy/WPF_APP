using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_APP.Models;
using WPF_APP.Utilities;
using System.Linq;
using System.Collections.Generic;

namespace WPF_APP.ViewModel
{
    public class TeacherVM : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<Teacher> _teachers;
        private Teacher _selectedTeacher;
        private ICommand _deleteCommand;
        private List<Teacher> _allTeachers;
        private string _searchText;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set => SetField(ref _teachers, value);
        }

        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                SetField(ref _selectedTeacher, value);
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
                        (param) => SelectedTeacher != null
                    );
                }
                return _deleteCommand;
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

        public TeacherVM()
        {
            _httpClient = new HttpClient();
            Teachers = new ObservableCollection<Teacher>();
            _allTeachers = new List<Teacher>();

            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        public async Task LoadTeachersAsync(string authToken)
        {
            try
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

                    // Сохраняем оригинальные данные
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allTeachers.Clear();
                        if (teachers != null)
                        {
                            _allTeachers.AddRange(teachers);
                        }
                        ApplyFilter();  // Применяем фильтр (если есть текст поиска)
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var cusmomWindow = new CustomMessageBox($"Ошибка сервера: {response.StatusCode}\n{errorContent}");
                    cusmomWindow.Show();
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var cusmomWindow = new CustomMessageBox($"Ошибка при загрузке: {ex.Message}");
                    cusmomWindow.Show();
                });
            }
        }

        public async Task DeleteTeacherAsync(string authToken, int teacherId)
        {
            try
            {
                // Очищаем старые заголовки
                _httpClient.DefaultRequestHeaders.Clear();

                // Добавляем токен
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                // Добавляем Accept заголовок
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Отправляем DELETE запрос
                var response = await _httpClient.DeleteAsync($"api/Teacher/{teacherId}");

                if (response.IsSuccessStatusCode)
                {
                    // Удаляем из оригинальной коллекции
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var teacherToRemove = _allTeachers.FirstOrDefault(t => t.Id == teacherId);
                        if (teacherToRemove != null)
                        {
                            _allTeachers.Remove(teacherToRemove);
                            ApplyFilter();  // Обновляем отображение
                            SelectedTeacher = null;
                            var successWindow = new CustomMessageBox("Учитель успешно удален");
                            successWindow.Show();
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
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var cusmomWindow = new CustomMessageBox($"Ошибка при удалении: {ex.Message}");
                    cusmomWindow.Show();
                });
            }
        }

        private async Task ExecuteDelete(object parameter)
        {
            try
            {
                if (SelectedTeacher == null)
                {
                    var warningWindow = new CustomMessageBox("Выберите учителя для удаления");
                    warningWindow.Show();
                    return;
                }

                // Получаем токен
                var authToken = Application.Current.Properties["AuthToken"]?.ToString();
                if (string.IsNullOrEmpty(authToken))
                {
                    var errorWindow = new CustomMessageBox("Ошибка авторизации: токен не найден");
                    errorWindow.Show();
                    return;
                }

                // Подтверждение удаления
                MessageBoxResult result = MessageBox.Show(
            $"Вы действительно хотите удалить учителя {SelectedTeacher.LastName} {SelectedTeacher.FirstName}?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await DeleteTeacherAsync(authToken, SelectedTeacher.Id);
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
            Teachers.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Показываем всех учителей
                foreach (var teacher in _allTeachers)
                {
                    Teachers.Add(teacher);
                }
            }
            else
            {
                // Ищем по фамилии, имени, отчеству и предметам
                var filtered = _allTeachers.Where(teacher =>
                    teacher.LastName?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    teacher.FirstName?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    teacher.Surename?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (teacher.Subjects != null && teacher.Subjects.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();

                foreach (var teacher in filtered)
                {
                    Teachers.Add(teacher);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}