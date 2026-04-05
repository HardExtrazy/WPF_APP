using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // Добавьте этот using
using WPF_APP.Models;

namespace WPF_APP.Services
{
    public class AuthService
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

        public static string Token { get; private set; }
        public static UserInfo CurrentUser { get; private set; }

        public async Task<LoginResponse> LoginAsync(string login, string password)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                // Подготавливаем запрос
                var loginRequest = new { login, password };
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Отправляем
                var response = await client.PostAsync($"{BaseUrl}/Auth/login", content);

                // Если не успешно - возвращаем ошибку
                if (!response.IsSuccessStatusCode)
                {
                    return CreateErrorResponse($"Ошибка сервера: {response.StatusCode}");
                }

                // Читаем ответ
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

                // Сохраняем токен при успехе
                if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
                {
                    Token = result.Token;
                    CurrentUser = result.User;

                    // СОХРАНЯЕМ ТОКЕН В Application.Current.Properties
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Properties["AuthToken"] = result.Token;
                        if (result.User != null)
                        {
                            Application.Current.Properties["CurrentUser"] = result.User;
                        }
                    });
                }

                return result ?? CreateErrorResponse("Неверный формат ответа сервера");
            }
        }

        private LoginResponse CreateErrorResponse(string message)
        {
            return new LoginResponse
            {
                Success = false,
                Message = message,
                Token = "",
                User = new UserInfo()
            };
        }

        // Добавьте метод для получения токена
        public static string GetToken()
        {
            if (Application.Current.Properties.Contains("AuthToken"))
            {
                return Application.Current.Properties["AuthToken"]?.ToString();
            }
            return Token;
        }

        // Добавьте метод для очистки токена (при выходе)
        public static void ClearToken()
        {
            Token = null;
            CurrentUser = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Properties.Remove("AuthToken");
                Application.Current.Properties.Remove("CurrentUser");
            });
        }
    }
}