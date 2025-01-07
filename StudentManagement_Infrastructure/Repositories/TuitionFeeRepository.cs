using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;
using StudentManagement_Domain.Interface;
using StudentManagement_Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Infrastructure.Repositories
{
    public class TuitionFeeRepository : ITuitionFeeRepository
    {
        private readonly AppDbContext _context;

        public TuitionFeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TuitionFee>> GetTuitionFeesByStudentIdAsync(int studentId)
        {
            return await _context.TuitionFees
                .Where(t => t.StudentID == studentId)
                .ToListAsync();
        }

        public async Task<TuitionFee> CreateTuitionFeeAsync(TuitionFeeDTO tuitionFeeDTO)
        {
            var tuitionFee = new TuitionFee
            {
                StudentID = tuitionFeeDTO.StudentID,
                Semester = tuitionFeeDTO.Semester,
                Year = tuitionFeeDTO.Year,
                Amount = tuitionFeeDTO.Amount,
                PaymentStatus = tuitionFeeDTO.PaymentStatus,
                DueDate = tuitionFeeDTO.DueDate,
                PaymentDate = tuitionFeeDTO.PaymentDate
            };

            await _context.TuitionFees.AddAsync(tuitionFee);
            await _context.SaveChangesAsync();
            return tuitionFee;
        }

        public async Task<TuitionFee> UpdateTuitionFeeAsync(TuitionFeeDTO tuitionFeeDTO)
        {
            var tuitionFee = await _context.TuitionFees.FirstOrDefaultAsync(t => t.TuitionFeeID == tuitionFeeDTO.TuitionFeeID);
            if (tuitionFee == null) throw new KeyNotFoundException("TuitionFee not found.");

            tuitionFee.Semester = tuitionFeeDTO.Semester;
            tuitionFee.Year = tuitionFeeDTO.Year;
            tuitionFee.Amount = tuitionFeeDTO.Amount;
            tuitionFee.PaymentStatus = tuitionFeeDTO.PaymentStatus;
            tuitionFee.DueDate = tuitionFeeDTO.DueDate;
            tuitionFee.PaymentDate = tuitionFeeDTO.PaymentDate;

            _context.TuitionFees.Update(tuitionFee);
            await _context.SaveChangesAsync();
            return tuitionFee;
        }

        public async Task<bool> DeleteTuitionFeeAsync(int tuitionFeeId)
        {
            var tuitionFee = await _context.TuitionFees.FirstOrDefaultAsync(t => t.TuitionFeeID == tuitionFeeId);
            if (tuitionFee == null) throw new KeyNotFoundException("TuitionFee not found.");

            _context.TuitionFees.Remove(tuitionFee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentTuitionFeeStatusDTO>> GetStudentsWithOverdueFeesByClassIdAsync(int classId)
        {
            var overdueStudents = await _context.Enrollments
                .Where(e => e.ClassID == classId)
                .Join(_context.Students,
                    enrollment => enrollment.StudentID,
                    student => student.StudentID,
                    (enrollment, student) => new { enrollment, student })
                .Join(_context.TuitionFees,
                    combined => combined.enrollment.StudentID,
                    tuitionFee => tuitionFee.StudentID,
                    (combined, tuitionFee) => new { combined.enrollment, combined.student, tuitionFee })
                .Where(data => data.tuitionFee.DueDate != null && data.tuitionFee.PaymentStatus != "paid" && data.tuitionFee.DueDate < DateTime.Now)
                .Select(data => new StudentTuitionFeeStatusDTO
                {
                    StudentID = data.student.StudentID,
                    FullName = $"{data.student.FirstName} {data.student.LastName}",
                    ClassID = data.enrollment.ClassID,
                    Semester = data.tuitionFee.Semester,
                    Year = data.tuitionFee.Year,
                    Amount = data.tuitionFee.Amount,
                    DueDate = data.tuitionFee.DueDate,
                    PaymentStatus = data.tuitionFee.PaymentStatus
                })
                .ToListAsync();

            return overdueStudents;
        }

    }
}
