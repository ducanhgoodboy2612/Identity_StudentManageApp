using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface IEnrollmentRepository
    {
        //Task<List<Student_EnrollmentDateDto>> GetStudentsByClassIdAsync(int classId);

        public IEnumerable<Student_EnrollmentDateDto> GetStudentsByClassIdAsync(int classId, out int total);
        Task<Enrollment> GetEnrollmentAsync(int studentId, int classId);
        Task<bool> CheckClassSlotAvailableAsync(int classId);
        Task<int> GetTotalCreditsForSemesterAsync(int studentId, int year, string semester);
        Task<Enrollment> CreateEnrollmentAsync(EnrollmentDTO enrollmentDTO);
        Task<bool> SoftDeleteEnrollmentAsync(int id);
        Task<(IEnumerable<EnrollmentDTO>, int)> GetPagedEnrollmentsByStudentIdAsync(int studentId, int page, int pageSize);

    }
}
