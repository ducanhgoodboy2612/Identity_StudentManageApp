using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class ClassDto
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int CourseID { get; set; }
        public int LecturerID { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
    }
}
