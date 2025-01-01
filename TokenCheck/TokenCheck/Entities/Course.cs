using System.Security.Claims;

namespace TokenCheck.Entities
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        // Quan hệ 1-n với Class
        public ICollection<Class> Classes { get; set; }
    }

}
