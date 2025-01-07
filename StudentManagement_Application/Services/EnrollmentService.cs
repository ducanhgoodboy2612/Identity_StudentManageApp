
using Microsoft.Extensions.Configuration;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
using System.Net.WebSockets;
namespace StudentManagement_Application.Services

{
    public class EnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IConfiguration _configuration;

        public EnrollmentService(IEnrollmentRepository enrollmentRepo, IConfiguration configuration)
        {
            _enrollmentRepo = enrollmentRepo;
            _configuration = configuration;
        }

        public async Task<string> CreateEnrollment(EnrollmentDTO enrollmentDTO)
        {
            var studentId = enrollmentDTO.StudentID;
            var classId = enrollmentDTO.ClassID;
            var enrollmentDate = enrollmentDTO.EnrollmentDate;
            var classAvailable = await _enrollmentRepo.CheckClassSlotAvailableAsync(classId);
            if (!classAvailable)
            {
                return "Class slot is full.";
            }

            var semester = GetSemester(enrollmentDate);
            var maxCredits = _configuration.GetValue<int>("AppSettings:MaxCreditsPerSemester");

            var totalCredits = await _enrollmentRepo.GetTotalCreditsForSemesterAsync(studentId, enrollmentDate.Year, semester);
            if (totalCredits >= maxCredits)
            {
                return $"Student has exceeded the maximum allowed credits ({maxCredits}) for this semester.";
            }

            var enrollment = new EnrollmentDTO
            {
                StudentID = studentId,
                ClassID = classId,
                EnrollmentDate = enrollmentDate,
                Status = 1,
            };

            await _enrollmentRepo.CreateEnrollmentAsync(enrollment);
            return "Enrollment successful.";
        }

        private string GetSemester(DateTime date)
        {
            if (date.Month >= 7 && date.Month <= 12)
            {
                return "Semester1"; // Kỳ 1 từ tháng 7 đến tháng 12
            }
            return "Semester2"; // Kỳ 2 từ tháng 1 đến tháng 6
        }
    }
}
