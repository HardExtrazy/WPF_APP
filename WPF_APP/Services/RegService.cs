using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_APP.Model;

namespace WPF_APP.Services
{
    public class RegService
    {
        private readonly HttpClient _httpClient;

        public RegService(string baseUrl)
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

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "Auth/register",
                    request,
                    jsonOptions
                );

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<RegisterResponse>(content, jsonOptions);
                        return result ?? new RegisterResponse
                        {
                            IsSuccess = false,
                            Message = "Не удалось обработать ответ сервера"
                        };
                    }
                    catch (JsonException)
                    {
                        return new RegisterResponse
                        {
                            IsSuccess = true,
                            Message = content
                        };
                    }
                }
                else
                {
                    return new RegisterResponse
                    {
                        IsSuccess = false,
                        Message = $"Ошибка сервера: {response.StatusCode}",
                        Errors = { content }
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Ошибка подключения к серверу",
                    Errors = { ex.Message }
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Произошла непредвиденная ошибка",
                    Errors = { ex.Message }
                };
            }
        }
    }
}