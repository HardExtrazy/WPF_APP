using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
    public class LessonGenerationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int LessonsCreated { get; set; }
        public int LessonsDeleted { get; set; }
        public List<GeneratedLessonInfo> CreatedLessons { get; set; } = new List<GeneratedLessonInfo>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
