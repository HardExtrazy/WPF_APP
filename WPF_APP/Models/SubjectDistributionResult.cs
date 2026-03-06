using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
     public class SubjectDistributionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AssignedLesson> AssignedLessons { get; set; } = new List<AssignedLesson>();
        public List<TeacherLoad> TeacherLoads { get; set; } = new List<TeacherLoad>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
