using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
using StudentManagement_Infrastructure.Repositories;
using StudentManagement_Domain.DTOs;
namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly LecturerRepository _lecturerRepository;

        public LecturerController(LecturerRepository service)
        {
            _lecturerRepository = service;
        }

        [Authorize(Policy = "ManageStudentsPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLecturerById(int id)
        {
            var lecturer = await _lecturerRepository.GetByIdAsync(id);
            if (lecturer == null) return NotFound();
            return Ok(lecturer);
        }

        [Authorize(Policy = "ManageUserPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> AddLecturer([FromBody] LecturerDTO lecturerDTO)
        {
            if (lecturerDTO == null)
            {
                return BadRequest("Lecturer data is required.");
            }

            try
            {
                var addedLecturer = await _lecturerRepository.AddLecturerAsync(lecturerDTO);
                return CreatedAtAction(nameof(GetLecturerById), new { id = addedLecturer.LecturerID }, addedLecturer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "ManageUserPolicy")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateLecturer([FromBody] LecturerDTO lecturerDTO)
        {
            if (lecturerDTO == null || lecturerDTO.LecturerID <= 0)
            {
                return BadRequest("Valid Lecturer data is required.");
            }

            try
            {
                var updatedLecturer = await _lecturerRepository.UpdateLecturerAsync(lecturerDTO);
                return Ok(updatedLecturer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var isDeleted = await _lecturerRepository.DeleteLecturerAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}
