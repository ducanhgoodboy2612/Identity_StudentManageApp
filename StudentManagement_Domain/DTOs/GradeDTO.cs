using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class GradeDTO
    {
        public int? GradeID { get; set; } // Optional for Update
        public int EnrollmentID { get; set; }
        public int ExamID { get; set; }
        public decimal MarksObtained { get; set; }
        public int StudentID { get; set; }
        public string Note { get; set; }
    }



    public class GradeDetailsDto
    {
        public int GradeID { get; set; }
        public decimal MarksObtained { get; set; }
        public string Note { get; set; }
        public int ExamID { get; set; }
        public string ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
        public decimal TotalMarks { get; set; }
    }

    public class GradeSummaryDto
    {
        public int StudentID { get; set; }
        public List<GradeDetailsDto> Grades { get; set; }
        public decimal AverageMarks { get; set; }
    }

    public class StudentGradesResult
    {
        public string FullName { get; set; }
        public List<ClassGradeResult> Grades { get; set; }
    }

    public class ClassGradeResult
    {
        public int ClassID { get; set; }
        public Dictionary<string, ExamTypeResult> GradesByExamType { get; set; }
        public decimal AverageMarks { get; set; }
    }

    public class ExamTypeResult
    {
        public decimal TotalMarks { get; set; }
        public decimal MarksObtained { get; set; }
    }

    public class StudentRetakeInfo
    {
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public decimal AverageGrade { get; set; }
    }

}
