using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_APP.Models.Dto
{
    public class ClassResponseDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Letter { get; set; }
        public int? Mentor { get; set; }
        public bool Shift { get; set; }
        public int WorkLoad { get; set; }
    }
}
