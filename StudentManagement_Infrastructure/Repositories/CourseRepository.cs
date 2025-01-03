using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> SearchCoursesAsync(string? courseName, int? departmentId)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(courseName))
            {
                query = query.Where(c => c.CourseName.Contains(courseName));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentID == departmentId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        // Thêm mới khóa học
        public async Task<Course> AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return false;
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
