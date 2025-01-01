namespace TokenCheck.Entities
{
    public class Grade
    {
        public int GradeID { get; set; }
        //public int EnrollmentID { get; set; }
        public int ExamID { get; set; }
        public decimal MarksObtained { get; set; }
        public int StudentID { get; set; }

        public string Note { get; set; }

        // FK to Enrollment
        public Enrollment Enrollment { get; set; }

        public Exam Exam { get; set; }
    }

}
