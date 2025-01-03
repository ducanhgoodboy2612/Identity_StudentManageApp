using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerRepository _service;

        public LecturerController(ILecturerRepository service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLecturerById(int id)
        {
            var lecturer = await _service.GetByIdAsync(id);
            if (lecturer == null) return NotFound();
            return Ok(lecturer);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateLecturer([FromBody] LecturerDTO lecturerDTO)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var createdLecturer = await _service.AddLecturerAsync(lecturerDTO);
        //    return CreatedAtAction(nameof(GetLecturerById), new { id = createdLecturer.LecturerID }, createdLecturer);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateLecturer(int id, [FromBody] LecturerDTO lecturerDTO)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var updatedLecturer = await _service.UpdateLecturerAsync(id, lecturerDTO);
        //    if (updatedLecturer == null) return NotFound();

        //    return Ok(updatedLecturer);
        //}



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var isDeleted = await _service.DeleteLecturerAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}
