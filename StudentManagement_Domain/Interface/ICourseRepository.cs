using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
    }
}
