using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
    public class TeacherLoadModel
    {
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public string Subgroup { get; set; }
        public int WeeklyHours { get; set; }
    }
}
