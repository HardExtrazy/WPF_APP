using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models.Dto
{
    public class TeacherCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surename { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int EducationId { get; set; }
        public int RoleId { get; set; }

        // Опциональные поля
        public string Login { get; set; }
        public string HashPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public List<int> SelectedSubjectIds { get; set; } = new List<int>();
    }
}
