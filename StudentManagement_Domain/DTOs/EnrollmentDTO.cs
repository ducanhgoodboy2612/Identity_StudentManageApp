using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class Student_EnrollmentDateDto
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; } // 1: Male, 0: Female
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class EnrollmentDTO
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Status { get; set; }

    }
}
