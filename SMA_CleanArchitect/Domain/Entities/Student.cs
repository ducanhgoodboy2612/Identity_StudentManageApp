using System.Diagnostics;

namespace Domain.Entities
{
    public class Student
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; } // 1: Male, 0: Female
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        //1 - n relation

        public ICollection<Enrollment> Enrollments { get; set; }

        public ICollection<TuitionFee> TuitionFees { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<Grade> Grades { get; set; }
    }

}
