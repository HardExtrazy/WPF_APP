using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models
{
    public class ClassResponse
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Letter { get; set; }
        public int? Mentor { get; set; }
        public string MentorName { get; set; }
        public bool Shift { get; set; }
        public int WorkLoad { get; set; }
    }
}
