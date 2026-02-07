using System.Text.Json.Serialization;

namespace WPF_APP.Model
{
    public class RegisterRequest
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("surename")]
        public string Surename { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("dateOfBirth")]
        public string DateOfBirth { get; set; } = string.Empty; 

        [JsonPropertyName("educationId")]
        public int EducationId { get; set; }

        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }
    }
}