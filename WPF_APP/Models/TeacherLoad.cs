using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
    public class TeacherLoad
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public bool IsClassTeacher { get; set; }
        public int CurrentLoad { get; set; }
        public int MaxLoad { get; set; }
        public List<TeacherSubject> Subjects { get; set; } = new List<TeacherSubject>();
    }
}
