using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPF_APP.Models;

namespace WPF_APP.Services
{
    /// <summary>
    /// Сервис для работы с распределением учителей-предметников
    /// </summary>
    public class SubjectTeacherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SubjectTeacherService()
        {
            _baseUrl = "https://localhost:7094/api/"; // Замените на ваш URL
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // Больше времени на сложные вычисления
        }

        /// <summary>
        /// Распределение учителей-предметников
        /// </summary>
        public async Task<SubjectDistributionResult> DistributeSubjectTeachersAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Отправка запроса на {_baseUrl}Distribution/subject-teachers");

                var response = await _httpClient.PostAsync("Distribution/subject-teachers", null);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Статус: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Ответ: {content}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubjectDistributionResult>();
                    return result ?? new SubjectDistributionResult { Success = false, Message = "Пустой ответ от сервера" };
                }
                else
                {
                    return new SubjectDistributionResult
                    {
                        Success = false,
                        Message = $"Ошибка сервера: {response.StatusCode}",                      
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                return new SubjectDistributionResult
                {
                    Success = false,
                    Message = $"Ошибка соединения: {ex.Message}"
                };
            }
        }
    }
}