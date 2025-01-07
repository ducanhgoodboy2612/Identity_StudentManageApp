using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassRepository _res;

        public ClassController(IClassRepository res)
        {
            _res = res;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchClasses([FromQuery] string courseName, [FromQuery] string semester, [FromQuery] int? year)
        {
            try
            {
                var classes = await _res.SearchClasses(courseName, semester, year);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
        [Authorize(Policy = "ManageStudentsPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            try
            {
                var classEntity = await _res.GetClassById(id);
                if (classEntity == null)
                {
                    return NotFound(new { message = "Class not found." });
                }

                return Ok(classEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpGet("get-by-course/{courseId}")]
        public async Task<IActionResult> GetClassesByCourseId(int courseId)
        {
            var classes = await _res.GetClassesByCourseIdAsync(courseId);
            if (classes == null || !classes.Any())
            {
                return NotFound("Không tìm thấy lớp nào cho CourseID được cung cấp.");
            }

            return Ok(classes);
        }

        [Authorize(Policy = "ManageTP_ClassPolicy")]
        [HttpPost]
        public async Task<IActionResult> AddClass([FromBody] ClassDto classDto)
        {
            try
            {
                var createdClass = await _res.AddClass(classDto);
                return CreatedAtAction(nameof(GetClassById), new { id = createdClass.ClassID }, createdClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] ClassDto classDto)
        {
            if (id != classDto.ClassID)
            {
                return BadRequest(new { message = "ClassID mismatch." });
            }

            try
            {
                var updatedClass = await _res.UpdateClass(classDto);
                if (updatedClass == null)
                {
                    return NotFound(new { message = "Class not found." });
                }

                return Ok(updatedClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                var result = await _res.DeleteClass(id);
                if (!result)
                {
                    return NotFound(new { message = "Class not found or no students associated." });
                }

                return Ok(new { message = "Class deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
    }
}
