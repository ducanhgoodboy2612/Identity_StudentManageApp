using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetSchedulesByClassAndDateRange([FromQuery] int? classId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var schedules = await _scheduleRepository.GetSchedulesByClass(classId, startDate, endDate);

            if (schedules == null || !schedules.Any())
            {
                return NotFound(new { message = "No schedules found for the given criteria" });
            }

            return Ok(schedules);
        }

        [HttpGet("GetSchedulesByStudentId")]
        public async Task<IActionResult> GetSchedulesByStudentIdAndDateRange(int studentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var schedules = await _scheduleRepository.GetSchedulesByStudentId(studentId, startDate, endDate);
                return Ok(new
                {
                    Total = schedules.Count,
                    Data = schedules
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
