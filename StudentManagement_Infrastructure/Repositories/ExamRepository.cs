using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Exam>> GetExamsByStudentId(int studentId, DateTime startDate, DateTime endDate)
        {
            var exams = await (from exam in _context.Exams
                               join enrollment in _context.Enrollments on exam.ClassID equals enrollment.ClassID
                               where enrollment.StudentID == studentId
                                     && exam.ExamDate >= startDate
                                     && exam.ExamDate <= endDate
                               select exam)
                              .ToListAsync();

            return exams;
        }

        public async Task<Exam> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .FirstOrDefaultAsync(e => e.ExamID == examId);
        }

        public async Task<List<Exam>> GetExamsByClassIdAsync(int classId)
        {
            return await _context.Exams
                .Where(e => e.ClassID == classId)
                .ToListAsync();
        }


        public async Task<Exam> AddExamAsync(ExamDTO examDTO)
        {
            var exam = new Exam
            {
                ClassID = examDTO.ClassID,
                ExamDate = examDTO.ExamDate,
                ExamType = examDTO.ExamType,
                TotalMarks = examDTO.TotalMarks
            };

            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<Exam> UpdateExamAsync(ExamDTO examDTO)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamID == examDTO.ExamID);
            if (exam == null) return null;

            exam.ClassID = examDTO.ClassID;
            exam.ExamDate = examDTO.ExamDate;
            exam.ExamType = examDTO.ExamType;
            exam.TotalMarks = examDTO.TotalMarks;

            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamID == id);
            if (exam == null) return false;

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    

}
