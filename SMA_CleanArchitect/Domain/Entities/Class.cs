namespace Domain.Entities
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int CourseID { get; set; }
        public int LecturerID { get; set; }
        public int RemainSlots { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }

        // Khóa ngoại tới Course
        public Course Course { get; set; }

        // Khóa ngoại tới Lecturer
        public Lecturer Lecturer { get; set; }

        // Quan hệ n-n với Student qua Enrollment
        public ICollection<Enrollment> Enrollments { get; set; }

        // Quan hệ 1-1 với Schedule
        public Schedule Schedule { get; set; }

        // Quan hệ 1-n với Attendance
        public ICollection<Attendance> Attendances { get; set; }

        // Quan hệ 1-n với Exam
        public ICollection<Exam> Exams { get; set; }
    }

}
