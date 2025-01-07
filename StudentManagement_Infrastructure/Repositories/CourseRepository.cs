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

        public async Task<Course> CreateCourseAsync(CourseDTO courseDTO)
        {
            var course = new Course
            {
                CourseName = courseDTO.CourseName,
                Credits = courseDTO.Credits,
                StartDate = courseDTO.StartDate,
                EndDate = courseDTO.EndDate,
                DepartmentID = courseDTO.DepartmentID
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }


        public async Task<Course> UpdateCourseAsync(CourseDTO courseDTO)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseDTO.CourseID);
            if (course == null) throw new KeyNotFoundException("Course not found.");

            course.CourseName = courseDTO.CourseName;
            course.Credits = courseDTO.Credits;
            course.StartDate = courseDTO.StartDate;
            course.EndDate = courseDTO.EndDate;
            course.DepartmentID = courseDTO.DepartmentID;

            _context.Courses.Update(course);
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
