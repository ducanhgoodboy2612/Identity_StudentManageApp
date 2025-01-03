using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class ExamDTO
    {
        public int ExamID { get; set; }
        public int ClassID { get; set; }
        public DateTime? ExamDate { get; set; }
        public string ExamType { get; set; }
        public decimal TotalMarks { get; set; }
    }
}
