using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class StudentDTO
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; } // 1: Male, 0: Female
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }


    public class StudentExamInfoDto
    {
        public string FullName { get; set; }
        public List<ExamInfoDto> Exams { get; set; }
    }

    public class ExamInfoDto
    {
        public DateTime? ExamDate { get; set; }
        public string ExamType { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal? MarksObtained { get; set; }
        public int ClassID { get; set; }
    }
}
