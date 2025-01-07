using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
using Microsoft.Extensions.Configuration;

namespace StudentManagement_Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;


        public EnrollmentRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public IEnumerable<Student_EnrollmentDateDto> GetStudentsByClassIdAsync(int classId, out int total)
        {
            var query = from enrollment in _context.Enrollments
                        join student in _context.Students on enrollment.StudentID equals student.StudentID
                        where enrollment.ClassID == classId
                        select new Student_EnrollmentDateDto
                        {
                            StudentID = student.StudentID,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Gender = student.Gender,
                            BirthDate = student.BirthDate,
                            Email = student.Email,
                            Phone = student.Phone,
                            Address = student.Address,
                            EnrollmentDate = enrollment.EnrollmentDate
                        };
            total = query.Count();

            return query.ToList();
        }

        public async Task<Enrollment> GetEnrollmentAsync(int studentId, int classId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentID == studentId && e.ClassID == classId);
        }

        public async Task<bool> CheckClassSlotAvailableAsync(int classId)
        {
            var classItem = await _context.Classes.FirstOrDefaultAsync(c => c.ClassID == classId);
            if (classItem == null) return false;

            return classItem.RemainSlots > 0;
        }

        public async Task<int> GetTotalCreditsForSemesterAsync(int studentId, int year, string semester)
        {
            var semesterStart = new DateTime(year, _configuration.GetValue<int>($"AppSettings:{semester}StartMonth"), 1);

            //var semesterEndYear = semester == "Semester1" ? year + 1 : year;

            var semesterEnd = new DateTime(year, _configuration.GetValue<int>($"AppSettings:{semester}EndMonth"), 30);

            var enrolledClasses = await _context.Enrollments
                .Where(e => e.StudentID == studentId && e.EnrollmentDate >= semesterStart && e.EnrollmentDate <= semesterEnd)
                .Select(e => e.ClassID)
                .ToListAsync();

            var totalCredits = await _context.Classes
                .Where(c => enrolledClasses.Contains(c.ClassID))
                .Join(_context.Courses, c => c.CourseID, cr => cr.CourseID, (c, cr) => cr.Credits)
                .SumAsync(credits => credits);

            return totalCredits;
        }

        public async Task<Enrollment> CreateEnrollmentAsync(EnrollmentDTO enrollmentDTO)
        {
            var enrollment = new Enrollment
            {
                StudentID = enrollmentDTO.StudentID,
                ClassID = enrollmentDTO.ClassID,
                EnrollmentDate = enrollmentDTO.EnrollmentDate,
                Status = enrollmentDTO.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> SoftDeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentID == id);
            if (enrollment == null) return false;

            enrollment.DeletedAt = DateTime.UtcNow;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<EnrollmentDTO>, int)> GetPagedEnrollmentsByStudentIdAsync(int studentId, int page, int pageSize)
        {
            var query = _context.Enrollments
                .Where(e => e.StudentID == studentId && e.DeletedAt == null)
                .OrderByDescending(e => e.EnrollmentDate);

            var total = await query.CountAsync();
            var enrollments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EnrollmentDTO
                {
                    StudentID = e.StudentID,
                    ClassID = e.ClassID,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return (enrollments, total);
        }
    }

    
}
