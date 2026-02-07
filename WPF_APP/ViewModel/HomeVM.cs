using System;
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
    public class HomeVM : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;

        private string _userId;
        private string _firstName;
        private string _lastName;
        private string _surename;
        private string _dateOfBirth;
        private string _email;
        private string _phoneNumber;
        private string _role;

        public event PropertyChangedEventHandler PropertyChanged;

        // Свойства для привязки в XAML
        public string UserId { get => _userId; set { if (_userId != value) { _userId = value; OnPropertyChanged(); } } }
        public string FirstName { get => _firstName; set { if (_firstName != value) { _firstName = value; OnPropertyChanged(); } } }
        public string LastName { get => _lastName; set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); } } }
        public string Surename { get => _surename; set { if (_surename != value) { _surename = value; OnPropertyChanged(); } } }
        public string DateOfBirth { get => _dateOfBirth; set { if (_dateOfBirth != value) { _dateOfBirth = value; OnPropertyChanged(); } } }
        public string Email { get => _email; set { if (_email != value) { _email = value; OnPropertyChanged(); } } }
        public string PhoneNumber { get => _phoneNumber; set { if (_phoneNumber != value) { _phoneNumber = value; OnPropertyChanged(); } } }
        public string Role { get => _role; set { if (_role != value) { _role = value; OnPropertyChanged(); } } }

        public HomeVM()
        {

                // Получаем URL из конфигурации
                var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

                // Создаем HttpClient
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };         
        }

        public async Task LoadUserDataAsync(string authToken)
        {          
                      
                // Очищаем старые заголовки
                _httpClient.DefaultRequestHeaders.Clear();

                // Добавляем токен в заголовок
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                // Добавляем Accept заголовок
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Отправляем запрос
                var response = await _httpClient.GetAsync("api/Auth/current-user");
                var content = await response.Content.ReadAsStringAsync();
                    // Десериализуем ответ
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var userData = JsonSerializer.Deserialize<UserResponse>(content, jsonOptions);


                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UserId = $"ID: {userData.Id.ToString()}";
                        FirstName = $"Имя: {userData.FirstName}"; 
                        LastName = $"Фамилия: {userData.LastName}";
                        Surename = $"Отчество: {userData.Surename}";
                        DateOfBirth = $"Дата рождения: {userData.DateOfBirth.ToString("dd.MM.yyyy")}";
                        Email = $"Почта: {userData.Email}";
                        PhoneNumber = $"Номер телефона: {userData.PhoneNumber}";
                    }); 
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}