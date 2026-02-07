using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    }
}