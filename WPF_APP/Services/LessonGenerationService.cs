using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPF_APP.Models;

namespace WPF_APP.Services
{
    /// <summary>
    /// Сервис для работы с генерацией уроков
    /// </summary>
    public class LessonGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public LessonGenerationService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Генерация уроков для начальной школы
        /// </summary>
        public async Task<LessonGenerationResult> GeneratePrimaryLessonsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Отправка запроса на {_baseUrl}LessonGeneration/primary");

                // Отправляем POST запрос
                var response = await _httpClient.PostAsync("LessonGeneration/primary", null);

                // Читаем ответ
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Статус: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Ответ: {content}");

                // Проверяем успешность
                if (response.IsSuccessStatusCode)
                {
                    // Десериализуем ответ
                    var result = await response.Content.ReadFromJsonAsync<LessonGenerationResult>();
                    return result ?? new LessonGenerationResult { Success = false, Message = "Пустой ответ от сервера" };
                }
                else
                {
                    // Обрабатываем ошибку
                    return new LessonGenerationResult
                    {
                        Success = false,
                        Message = $"Ошибка сервера: {response.StatusCode}",
                        Errors = new System.Collections.Generic.List<string> { content }
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                return new LessonGenerationResult
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }
    }
}