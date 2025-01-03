using Microsoft.Extensions.Configuration;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Application.Services
{
    public class GradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IExamRepository _examRepository;

        public GradeService(IGradeRepository gradeRepository, IExamRepository examRepository)
        {
            _gradeRepository = gradeRepository;
            _examRepository = examRepository;
        }

        public async Task<string> AddGradeAsync(int enrollmentId, int examId, decimal marksObtained)
        {
            var exam = await _examRepository.GetExamByIdAsync(examId);
            if (exam == null)
                return "Exam not found";

            var grade = new GradeDTO
            {
                EnrollmentID = enrollmentId,
                ExamID = examId,
                MarksObtained = marksObtained
            };

            if (exam.ExamType == "Final")
            {
                var isEligible = await _gradeRepository.CheckQuizAndMidtermScores(exam.ClassID, enrollmentId);

                if (!isEligible)
                {
                    grade.Note = "NMR"; // Not Met Requirements
                    await _gradeRepository.CreateGradeAsync(grade);
                    return "Cannot add grade for Final exam as prerequisites are not met.";
                }
            }

            // Add grade if valid
            await _gradeRepository.CreateGradeAsync(grade);

            if (exam.ExamType == "Final")
            {
                var classAverage = await _gradeRepository.GetAverageGrade(enrollmentId);
                grade.Note = marksObtained >= classAverage ? "Pass" : "Fail";
                //await _gradeRepository.UpdateGradeAsync(grade);
            }

            return "Grade added successfully.";
        }

        public async Task<Dictionary<int, GradeSummaryDto>> GetGradesAndAverages(int studentId, DateTime startDate, DateTime endDate)
        {
            var grades = await _gradeRepository.GetGradesByStudentId(studentId, startDate, endDate);

            var result = grades
               .GroupBy(x => x.Exam.ClassID)
               .ToDictionary(
                   g => g.Key,
                   g => new GradeSummaryDto
                   {
                       Grades = g.Select(grade => new GradeDetailsDto
                       {
                           GradeID = grade.GradeID,
                           MarksObtained = grade.MarksObtained,
                           Note = grade.Note,
                           ExamID = grade.ExamID,
                           ExamType = grade.Exam.ExamType,
                           ExamDate = grade.Exam.ExamDate,
                           TotalMarks = grade.Exam.TotalMarks
                       }).ToList(),
                       AverageMarks = g.Average(grade => grade.MarksObtained)
                   });

            return result;
        }
    }
}
