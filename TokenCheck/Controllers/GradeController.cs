using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
using StudentManagement_Application.Services;
using StudentManagement_Infrastructure.Repositories;
namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly IGradeRepository _repo;
        private readonly GradeService _service;


        public GradeController(IGradeRepository enrollmentRepo, GradeService service)
        {
            _repo = enrollmentRepo;
            _service = service;
        }

        [HttpGet("GetGrades")]
        public async Task<IActionResult> GetGradesByStudent([FromQuery] int studentId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _service.GetGradesAndAverages(studentId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("GetClassExamResults")]
        public async Task<IActionResult> GetClassExamResults(int classId)
        {
            try
            {
                var results = await _repo.GetExamResultsByClassId(classId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade(GradeDTO gradeDTO)
        {
            var grade = await _repo.CreateGradeAsync(gradeDTO);
            return CreatedAtAction(nameof(CreateGrade), new { id = grade.GradeID }, grade);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGrade(GradeDTO gradeDTO)
        {
            var grade = await _repo.UpdateGradeAsync(gradeDTO);
            return Ok(grade);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var isDeleted = await _repo.DeleteGradeAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }

        [HttpDelete("DeleteGrades")]
        public async Task<IActionResult> DeleteGrades(int studentId, int classId)
        {
            try
            {
                var isDeleted = await _repo.DeleteGradesByStudentAndClassAsync(studentId, classId);
                return Ok(new { message = "Xóa điểm thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa điểm.", error = ex.Message });
            }
        }


        [HttpGet("average-score")]
        public async Task<IActionResult> GetAverageScore(int classId, int studentId)
        {
            var averageScore = await _repo.CalculateAverageScoreAsync(classId, studentId);

            return Ok(new
            {
                ClassId = classId,
                StudentId = studentId,
                AverageScore = averageScore
            });
        }

        //[HttpGet("{studentId}/grades")]
        //public async Task<IActionResult> GetGradesByStudentId(int studentId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        //{
        //    // Validate inputs
        //    if (startDate > endDate)
        //    {
        //        return BadRequest(new { Message = "Start date must be earlier than or equal to end date." });
        //    }

        //    var grades = await _repo.GetGradesByStudentId(studentId, startDate, endDate);

        //    if (!grades.Any())
        //    {
        //        return NotFound(new { Message = "No grades found for the given student and date range." });
        //    }

        //    var result = grades.Select(g => new
        //    {
        //        g.GradeID,
        //        g.EnrollmentID,
        //        g.ExamID,
        //        g.MarksObtained,
        //        g.Note,
        //        Exam = new
        //        {
        //            g.Exam.ExamID,
        //            g.Exam.ClassID,
        //            g.Exam.ExamDate,
        //            g.Exam.ExamType,
        //            g.Exam.TotalMarks
        //        }
        //    });

        //    return Ok(result);
        //}

        //[HttpGet("{studentId}/grade-summary")]
        //public async Task<IActionResult> GetStudentGrades(int studentId)
        //{
        //    var result = await _repo.GetStudentGradesAsync(studentId);

        //    if (result == null)
        //    {
        //        return NotFound(new
        //        {
        //            Message = "Student not found or no grades available."
        //        });
        //    }

        //    return Ok(new
        //    {
        //        result.FullName,
        //        Grades = result.Grades.Select(g => new
        //        {
        //            g.ClassID,
        //            GradesByExamType = g.GradesByExamType.ToDictionary(
        //                e => e.Key,
        //                e => new
        //                {
        //                    e.Value.MarksObtained,
        //                    e.Value.TotalMarks
        //                }
        //            ),
        //            g.AverageMarks
        //        })
        //    });
        //}

        [HttpGet("{studentId}/retake-classes")]
        public async Task<IActionResult> GetRetakeClasses(int studentId)
        {
            var retakeClasses = await _repo.GetRetakeClassesAsync(studentId);

            if (!retakeClasses.Any())
            {
                return NotFound(new { Message = "No classes found that require retake for this student." });
            }

            return Ok(retakeClasses.Select(c => new
            {
                c.ClassID,
                c.ClassName,
                c.CourseID,
                c.LecturerID,
                c.Semester,
                c.Year
            }));
        }

        [HttpGet("{classId}/retaking-students")]
        public async Task<IActionResult> GetStudentsRetakingClass(int classId)
        {
            var studentsRetaking = await _repo.GetStudentsRetakingClassAsync(classId);

            if (!studentsRetaking.Any())
            {
                return NotFound(new { Message = "No students found retaking this class." });
            }

            return Ok(studentsRetaking);
        }
    }
}
