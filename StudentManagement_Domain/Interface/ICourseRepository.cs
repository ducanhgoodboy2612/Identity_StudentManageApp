using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public partial interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> SearchCoursesAsync(string? courseName, int? departmentId);
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<Course> CreateCourseAsync(CourseDTO courseDTO);
        Task<Course> UpdateCourseAsync(CourseDTO courseDTO);
        Task<bool> DeleteCourseAsync(int courseId);
    }
}
