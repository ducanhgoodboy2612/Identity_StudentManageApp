namespace StudentManagement_Domain.Entities
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } // Present, Absent, Late

        // Khóa ngoại tới Student
        public Student Student { get; set; }

        // Khóa ngoại tới Class
        public Class Class { get; set; }
    }

}
