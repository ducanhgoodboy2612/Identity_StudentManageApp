using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Infrastructure.Repositories
{
    public class LecturerRepository : ILecturerRepository
    {
        private readonly AppDbContext _context;

        public LecturerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lecturer> GetByIdAsync(int id)
        {
            return await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerID == id);
        }

        public async Task<Lecturer> AddLecturerAsync(LecturerDTO lecturerDTO)
        {
            if (lecturerDTO == null)
                throw new ArgumentNullException(nameof(lecturerDTO));

            var lecturer = new Lecturer
            {
                FullName = lecturerDTO.FullName,
                Phone = lecturerDTO.Phone,
                Email = lecturerDTO.Email,
                YoB = lecturerDTO.YoB
            };

            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
            return lecturer;
        }

        public async Task<Lecturer> UpdateLecturerAsync(int id, LecturerDTO lecturerDTO)
        {
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerID == id);
            if (lecturer == null) return null;

            lecturer.FullName = lecturerDTO.FullName;
            lecturer.Phone = lecturerDTO.Phone;
            lecturer.Email = lecturerDTO.Email;
            lecturer.YoB = lecturerDTO.YoB;

            await _context.SaveChangesAsync();
            return lecturer;
        }


        public async Task<bool> DeleteLecturerAsync(int id)
        {
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerID == id);
            if (lecturer == null) return false;

            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class LecturerDTO
    {
        public int LecturerID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? YoB { get; set; }
    }
}
