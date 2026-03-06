using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPF_APP.Models;

namespace WPF_APP.Services
{
    /// <summary>
    /// Сервис для работы с уроками
    /// </summary>
    public class LessonService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public LessonService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        /// <summary>
        /// Получение нагрузки учителей для отображения в таблице
        /// </summary>
        public async Task<List<TeacherLoadModel>> GetTeacherLoadsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<TeacherLoadModel>>("Lesson/teacher-loads");
                return response ?? new List<TeacherLoadModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
                return new List<TeacherLoadModel>();
            }
        }

        public async Task<bool> ClearSchedualTableAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("Schedual/clear", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка очистки расписания: {ex.Message}");
                return false;
            }
        }
    }
}