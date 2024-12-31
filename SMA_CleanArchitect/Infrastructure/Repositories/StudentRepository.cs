using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.DTOs;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Student> Students, int TotalRecords)> SearchStudentsAsync(
            string? firstName,
            string? lastName,
            string? phone,
            int page,
            int pageSize)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(s => s.FirstName.Contains(firstName));
            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(s => s.LastName.Contains(lastName));
            if (!string.IsNullOrEmpty(phone))
                query = query.Where(s => s.Phone.Contains(phone));

            var totalRecords = await query.CountAsync();
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students, totalRecords);
        }

        public IEnumerable<Student> SearchStudentsAsync(string? name, string? phone, int page, int pageSize, out int total)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                var lowerName = name.ToLower();
                query = query.Where(s =>
                    (s.FirstName + " " + s.LastName).ToLower().Contains(lowerName) ||
                    (s.LastName + " " + s.FirstName).ToLower().Contains(lowerName));
            }

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(s => s.Phone.Contains(phone));

            total = query.Count();

            return query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToList();
        }




        public async Task<StudentExamInfoDto> GetStudentExamInfo(int studentId, DateTime startDate, DateTime endDate)
        {
            // Lấy thông tin sinh viên theo StudentID
            var student = await _context.Students
                .Include(s => s.Enrollments) // Bao gồm thông tin Enrollments
                    .ThenInclude(e => e.Grades) // Bao gồm thông tin Grades
                    .ThenInclude(g => g.Exam) // Bao gồm thông tin Exam trong Grades
                .FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null)
            {
                return null;
            }


            var exams = student.Enrollments
                .SelectMany(e => e.Grades)
                .Where(g => g.Exam.ExamDate >= startDate && g.Exam.ExamDate <= endDate)
                .Select(g => new ExamInfoDto
                {
                    ExamDate = g.Exam.ExamDate,
                    ExamType = g.Exam.ExamType,
                    TotalMarks = g.Exam.TotalMarks,
                    MarksObtained = g.MarksObtained,
                    ClassID = g.Exam.ClassID // Lấy thông tin ClassID của mỗi Exam
                })
                .ToList();


            return new StudentExamInfoDto
            {
                FullName = $"{student.FirstName} {student.LastName}",
                Exams = exams
            };
        }

        public async Task<List<Exam>> GetExamsSchedule(int studentId, int n)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(n);

            var exams = await _context.Exams
                .Where(e => e.ExamDate >= startDate && e.ExamDate <= endDate)
                .Join(_context.Enrollments,
                      exam => exam.ClassID,
                      enrollment => enrollment.ClassID,
                      (exam, enrollment) => new { exam, enrollment })
                .Where(x => x.enrollment.StudentID == studentId) // Lọc theo studentId
                .Select(x => x.exam)
                .ToListAsync();

            return exams;
        }

        public async Task<Student> GetbyId(int id)
        {
            var student = await _context.Students
                           .FirstOrDefaultAsync(s => s.StudentID == id);

            return student;
        }

        public async Task<Student> CreateStudent(StudentDTO studentDTO)
        {
            if (studentDTO == null)
                throw new ArgumentNullException(nameof(studentDTO));

            var student = new Student
            {
                FirstName = studentDTO.FirstName,
                LastName = studentDTO.LastName,
                Gender = studentDTO.Gender,
                BirthDate = studentDTO.BirthDate,
                Email = studentDTO.Email,
                Phone = studentDTO.Phone,
                Address = studentDTO.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }


        public async Task<Student> UpdateStudent(StudentDTO studentDTO)
        {
            if (studentDTO == null)
                throw new ArgumentNullException(nameof(studentDTO));

            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentID == studentDTO.StudentID);

            if (existingStudent == null)
                throw new KeyNotFoundException("Sinh viên không tồn tại.");

            existingStudent.FirstName = studentDTO.FirstName;
            existingStudent.LastName = studentDTO.LastName;
            existingStudent.Gender = studentDTO.Gender;
            existingStudent.BirthDate = studentDTO.BirthDate;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Phone = studentDTO.Phone;
            existingStudent.Address = studentDTO.Address;
            existingStudent.UpdatedAt = DateTime.UtcNow;

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
            return existingStudent;
        }


        public async Task<bool> SoftDeleteStudentAsync(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id);
            if (student == null) return false;

            student.DeletedAt = DateTime.UtcNow;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .ToListAsync();
        }
    }

    //public class StudentDTO
    //{
    //    public int StudentID { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public int Gender { get; set; } // 1: Male, 0: Female
    //    public DateTime? BirthDate { get; set; }
    //    public string Email { get; set; }
    //    public string Phone { get; set; }
    //    public string Address { get; set; }
    //}


    //public class StudentExamInfoDto
    //{
    //    public string FullName { get; set; }
    //    public List<ExamInfoDto> Exams { get; set; }
    //}

    //public class ExamInfoDto
    //{
    //    public DateTime? ExamDate { get; set; }
    //    public string ExamType { get; set; }
    //    public decimal TotalMarks { get; set; }
    //    public decimal? MarksObtained { get; set; }
    //    public int ClassID { get; set; }
    //}

}
