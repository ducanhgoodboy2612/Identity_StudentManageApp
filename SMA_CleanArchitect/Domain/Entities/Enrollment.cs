using System.Diagnostics;

namespace Domain.Entities
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Student Student { get; set; }

        public Class Class { get; set; }

        public ICollection<Grade> Grades { get; set; }
    }

}
