using System;

namespace WPF_APP.Models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Surename { get; set; }
        public DateTime DateOfBirth { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
    }
}