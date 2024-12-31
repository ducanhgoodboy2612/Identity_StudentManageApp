using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SMA_CleanArchitect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase

    {
        private readonly AppDbContext _context;

        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        [HttpPost("search")]
        public IActionResult SearchStudents([FromBody] Dictionary<string, object> formData)
        {
            string? name = formData.ContainsKey("name") ? formData["name"]?.ToString() : null;
            string? phone = formData.ContainsKey("phone") ? formData["phone"]?.ToString() : null;
            int page = formData.ContainsKey("page") ? int.Parse(formData["page"].ToString()!) : 1;
            int pageSize = formData.ContainsKey("pageSize") ? int.Parse(formData["pageSize"].ToString()!) : 10;

            int totalRecords;
            var students = _studentRepository.SearchStudentsAsync(name, phone, page, pageSize, out totalRecords);

            return Ok(new
            {
                TotalRecords = totalRecords,
                Students = students
            });
        }


        
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentRepository.GetAllStudentsAsync();
            return Ok(students);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {


            try
            {
                var student = await _studentRepository.GetbyId(id);
                return Ok(student);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Sinh viên không tồn tại.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest("Dữ liệu sinh viên không hợp lệ.");
            }

            var newStudent = await _studentRepository.CreateStudent(studentDTO);
            return Ok(newStudent);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentDTO studentDTO)
        {
            if (studentDTO.StudentID == null)
            {
                return BadRequest("Nhập Id sinh viên.");
            }

            try
            {
                var updatedStudent = await _studentRepository.UpdateStudent(studentDTO);
                return Ok(updatedStudent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Sinh viên không tồn tại.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteStudent(int id)
        {
            var isDeleted = await _studentRepository.SoftDeleteStudentAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }

        [HttpGet("student-exam-info")]
        public async Task<IActionResult> GetStudentExamInfo(int studentId, DateTime startDate, DateTime endDate)
        {
            var result = await _studentRepository.GetStudentExamInfo(studentId, startDate, endDate);

            if (result == null)
            {
                return NotFound($"No student found with ID {studentId}");
            }

            return Ok(result);
        }

        [HttpGet("GetExamsSchedule")]
        public async Task<IActionResult> GetExamsSchedule(int studentId, int n)
        {
            var exams = await _studentRepository.GetExamsSchedule(studentId, n);

            if (exams == null || exams.Count == 0)
            {
                return NotFound("Không có bài kiểm tra nào.");
            }

            return Ok(exams);
        }
    }
}
