using System;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WPF_APP.Services
{
    public class ReferenceService
    {
        private readonly HttpClient _httpClient;

        public ReferenceService(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));
            }

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<ReferenceData> GetRegisterReferenceDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Reference/register-data");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ReferenceData>(content, jsonOptions);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReferenceItem[]> GetEducationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Reference/educations");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ReferenceItem[]>(content, jsonOptions);
                }

                return new ReferenceItem[0];
            }
            catch
            {
                return new ReferenceItem[0];
            }
        }

        public async Task<ReferenceItem[]> GetRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Reference/roles");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ReferenceItem[]>(content, jsonOptions);
                }

                return new ReferenceItem[0];
            }
            catch
            {
                return new ReferenceItem[0];
            }
        }
    }

    public class ReferenceData
    {
        public ReferenceItem[] Educations { get; set; }
        public ReferenceItem[] Roles { get; set; }

        public ReferenceData()
        {
            Educations = new ReferenceItem[0];
            Roles = new ReferenceItem[0];
        }
    }

    public class ReferenceItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ReferenceItem()
        {
            Name = string.Empty;
        }
    }
}