using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository _examRepository;

        public ExamController(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        [HttpGet("GetbyId")]
        public async Task<IActionResult> GetExamsById(int id)
        {

            var exams = await _examRepository.GetExamByIdAsync(id);
            return Ok(exams);
        }

        [HttpGet("GetExamsByStudentId")]
        public async Task<IActionResult> GetExamsByStudentId(int studentId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be later than end date.");
            }

            var exams = await _examRepository.GetExamsByStudentId(studentId, startDate, endDate);
            return Ok(exams);
        }

        [HttpGet("GetExamsByClass")]
        public async Task<IActionResult> GetExamsByClass(int classId)
        {
            var exams = await _examRepository.GetExamsByClassIdAsync(classId);
            return Ok(exams);
        }


        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDTO examDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdExam = await _examRepository.AddExamAsync(examDTO);
            return CreatedAtAction(nameof(CreateExam), new { id = createdExam.ExamID }, createdExam);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateExam([FromBody] ExamDTO examDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedExam = await _examRepository.UpdateExamAsync(examDTO);
            if (updatedExam == null) return NotFound();

            return Ok(updatedExam);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var isDeleted = await _examRepository.DeleteExamAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}
