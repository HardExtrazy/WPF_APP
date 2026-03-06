using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
    public class AssignedLesson
    {
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public int Hours { get; set; }
        public int? SubgroupNumber { get; set; }
        public bool IsPrimary { get; set; }
    }
}
