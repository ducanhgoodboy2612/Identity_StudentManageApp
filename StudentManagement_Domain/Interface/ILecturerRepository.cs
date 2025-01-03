using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;

namespace StudentManagement_Domain.Interface
{
    public interface ILecturerRepository
    {
        Task<Lecturer> GetByIdAsync(int id);
        //Task<Lecturer> AddLecturerAsync(LecturerDTO lecturerDTO);
        //Task<Lecturer> UpdateLecturerAsync(int id, LecturerDTO lecturerDTO);
        Task<bool> DeleteLecturerAsync(int id);
    }
}
