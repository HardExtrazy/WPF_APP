using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WPF_APP.Model
{
    public class RegisterResponse
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; } 

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new List<string>();

        [JsonPropertyName("user")]
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}