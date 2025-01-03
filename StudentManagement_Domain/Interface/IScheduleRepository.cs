using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface IScheduleRepository
    {
        Task<List<Schedule>> GetSchedulesByClass(int? classId, DateTime? startDate, DateTime? endDate);
        Task<List<Schedule>> GetSchedulesByStudentId(int studentId, DateTime startDate, DateTime endDate);
    }
}
