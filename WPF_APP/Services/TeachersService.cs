using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using WPF_APP.Models;
using WPF_APP.Models.Dto;

namespace WPF_APP.Services
{
    public class TeachersService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TeachersService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<List<Education>> GetEducationsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Education>>("Teachers/education");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения образования: {ex.Message}");
                return new List<Education>();
            }
        }

        public async Task<List<Role>> GetTeacherRolesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Role>>("Teachers/roles");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения ролей: {ex.Message}");
                return new List<Role>();
            }
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Subject>>("Teachers/subjects");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения предметов: {ex.Message}");
                return new List<Subject>();
            }
        }

        public async Task<TeacherResponse> CreateTeacherAsync(TeacherCreateDto teacher)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Teachers", teacher);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TeacherResponse>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка создания учителя: {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания учителя: {ex.Message}");
                throw;
            }
        }
    }
}
