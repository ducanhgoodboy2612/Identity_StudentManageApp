using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TuitionFeeController : ControllerBase
    {
        private readonly ITuitionFeeRepository _tuitionFeeRepository;

        public TuitionFeeController(ITuitionFeeRepository tuitionFeeRepository)
        {
            _tuitionFeeRepository = tuitionFeeRepository;
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var tuitionFees = await _tuitionFeeRepository.GetTuitionFeesByStudentIdAsync(studentId);
            return Ok(tuitionFees);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TuitionFeeDTO tuitionFeeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdTuitionFee = await _tuitionFeeRepository.CreateTuitionFeeAsync(tuitionFeeDTO);
            return CreatedAtAction(nameof(GetByStudentId), new { studentId = createdTuitionFee.StudentID }, createdTuitionFee);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TuitionFeeDTO tuitionFeeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedTuitionFee = await _tuitionFeeRepository.UpdateTuitionFeeAsync(tuitionFeeDTO);
                return Ok(updatedTuitionFee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{tuitionFeeId}")]
        public async Task<IActionResult> Delete(int tuitionFeeId)
        {
            try
            {
                var success = await _tuitionFeeRepository.DeleteTuitionFeeAsync(tuitionFeeId);
                if (!success) return BadRequest(new { message = "Failed to delete TuitionFee." });

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("overdue/{classId}")]
        public async Task<IActionResult> GetOverdueStudentsByClassId(int classId)
        {
            var overdueStudents = await _tuitionFeeRepository.GetStudentsWithOverdueFeesByClassIdAsync(classId);
            if (!overdueStudents.Any())
                return NotFound(new { message = "No overdue tuition fees found for this class." });

            return Ok(overdueStudents);
        }
    }
}
