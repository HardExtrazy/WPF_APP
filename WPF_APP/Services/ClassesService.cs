// WPF_APP/Services/ClassesService.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using WPF_APP.Models;
using WPF_APP.Models.Dto;

namespace WPF_APP.Services
{
    public class ClassesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ClassesService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);

            // Добавляем токен авторизации, если есть
            if (Application.Current.Properties["AuthToken"] is string token)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<NumberItem>> GetClassNumbersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<NumberItem>>("api/Classes/numbers")
                       ?? new List<NumberItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения номеров: {ex.Message}");
                return new List<NumberItem>();
            }
        }

        public async Task<List<LetterItem>> GetClassLettersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<LetterItem>>("api/Classes/letters")
                       ?? new List<LetterItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения букв: {ex.Message}");
                return new List<LetterItem>();
            }
        }

        public async Task<List<MentorItem>> GetAvailableMentorsAsync(int classNumber)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<MentorItem>>($"api/Classes/available-mentors?classNumber={classNumber}")
                       ?? new List<MentorItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения классных руководителей: {ex.Message}");
                return new List<MentorItem>();
            }
        }

        public async Task<ClassResponseDto> CreateClassAsync(ClassCreateDto classDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Classes", classDto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClassResponseDto>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка создания класса: {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания класса: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ClassResponseDto>>("api/Classes");

                if (response != null)
                {
                    // Преобразуем ответ сервера в модель для отображения
                    return response.Select(c => new Class
                    {
                        FullClassName = $"{c.Number}{c.Letter}",
                        Mentor = c.Mentor?.ToString() ?? "Не назначен", // Здесь нужно будет получить имя учителя
                        WorkLoad = c.WorkLoad,
                        Shift = c.Shift ? 1 : 2
                    }).ToList();
                }

                return new List<Class>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения классов: {ex.Message}");
                return new List<Class>();
            }
        }
    }
}