using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface IClassRepository
    {
        Task<IEnumerable<Class>> SearchClasses(string courseName, string semester, int? year);
        Task<Class> GetClassById(int classId);
        Task<ClassDto> AddClass(ClassDto classDto);
        Task<ClassDto> UpdateClass(ClassDto classDto);
        Task<bool> DeleteClass(int id);
        Task<IEnumerable<Class>> GetClassesByCourseIdAsync(int courseId);
    }
}
