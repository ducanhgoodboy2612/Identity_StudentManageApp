namespace TokenCheck.Entities
{
    public class TuitionFee
    {
        public int TuitionFeeID { get; set; }
        public int StudentID { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // Paid, Unpaid, Partial
        public DateTime? DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        // Khóa ngoại tới Student
        public Student Student { get; set; }
    }

}
