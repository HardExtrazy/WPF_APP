using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPF_APP.Models;

namespace WPF_APP.Services
{
    public class SubjectService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SubjectService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]+"/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {        
                return await _httpClient.GetFromJsonAsync<List<Subject>>("Subject");
        }

        public async Task<List<StudyLoad>> GetStudyLoadsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<StudyLoad>>("Subject/studyloads")
                       ?? new List<StudyLoad>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                return new List<StudyLoad>();
            }
        }

        public async Task<bool> SaveStudyLoadsAsync(List<StudyLoadSave> loads)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Subject/save-studyloads", loads);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }
    }
}