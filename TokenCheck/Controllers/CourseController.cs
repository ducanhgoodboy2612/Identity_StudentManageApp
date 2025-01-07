using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _repository;

        public CourseController(ICourseRepository courseRepository)
        {
            _repository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _repository.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string? courseName, [FromQuery] int? departmentId)
        {
            var courses = await _repository.SearchCoursesAsync(courseName, departmentId);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            return Ok(course);
        }
        [Authorize(Policy = "ManageTP_ClassPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDTO courseDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCourse = await _repository.CreateCourseAsync(courseDTO);
            return CreatedAtAction(nameof(GetCourseById), new { id = createdCourse.CourseID }, createdCourse);
        }

        [Authorize(Policy = "ManageTP_ClassPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDTO courseDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCourse = await _repository.UpdateCourseAsync(courseDTO);
                return Ok(updatedCourse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "ManageTP_ClassPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await _repository.DeleteCourseAsync(id);
            if (!success)
                return NotFound(new { message = "Course not found or could not be deleted." });

            return NoContent();
        }
    }
}
