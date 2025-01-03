using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface IExamRepository
    {
        Task<List<Exam>> GetExamsByStudentId(int studentId, DateTime startDate, DateTime endDate);
        Task<Exam> GetExamByIdAsync(int examId);
        Task<List<Exam>> GetExamsByClassIdAsync(int classId);
        Task<Exam> AddExamAsync(ExamDTO examDTO);
        Task<Exam> UpdateExamAsync(ExamDTO examDTO);
        Task<bool> DeleteExamAsync(int id);
    }
}
