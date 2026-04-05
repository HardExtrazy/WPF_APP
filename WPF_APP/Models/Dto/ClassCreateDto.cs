using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models.Dto
{
    public class ClassCreateDto
    {
        public int Number { get; set; }
        public string Letter { get; set; }
        public int? MentorId { get; set; }
        public bool IsFirstShift { get; set; }
        public int WorkLoad { get; set; }
    }
}
