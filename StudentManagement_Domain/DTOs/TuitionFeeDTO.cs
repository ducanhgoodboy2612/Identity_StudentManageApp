using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class TuitionFeeDTO
    {
        public int TuitionFeeID { get; set; }
        public int StudentID { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // Paid, Unpaid, Partial
        public DateTime? DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class StudentTuitionFeeStatusDTO
    {
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public int ClassID { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string PaymentStatus { get; set; }
    }

}
