using Microsoft.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Infrastructure.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckQuizAndMidtermScores(int classId, int enrollmentId)
        {
            var exams = await _context.Exams
                .Where(e => e.ClassID == classId && (e.ExamType == "Quiz" || e.ExamType == "Midterm"))
                .Select(e => e.ExamID)
                .ToListAsync();

            if (exams.Count < 2) return false;

            var grades = await _context.Grades
                .Where(g => exams.Contains(g.ExamID) && g.EnrollmentID == enrollmentId)
                .Select(g => g.MarksObtained)
                .ToListAsync();

            if (grades.Count < 2) return false;

            var average = grades.Average();
            return average >= 5;
        }

        public async Task<decimal> CalculateAverageScoreAsync(int classId, int studentId)
        {
            var exams = await (from grade in _context.Grades
                               join exam in _context.Exams on grade.ExamID equals exam.ExamID
                               where exam.ClassID == classId && grade.StudentID == studentId
                               select new
                               {
                                   grade.MarksObtained,
                                   exam.TotalMarks,
                                   exam.ExamType
                               }).ToListAsync();

            if (exams == null || exams.Count == 0)
                return 0;

            decimal totalWeightedScore = exams.Sum(e =>
                e.MarksObtained / e.TotalMarks * 10 * (e.ExamType == "Final" ? 2 : 1));
            int totalExams = exams.Count + exams.Count(e => e.ExamType == "Final");

            return totalWeightedScore / totalExams;
        }

        public async Task<Grade> CreateGradeAsync(GradeDTO gradeDTO)
        {
            var grade = new Grade
            {
                EnrollmentID = gradeDTO.EnrollmentID,
                ExamID = gradeDTO.ExamID,
                MarksObtained = gradeDTO.MarksObtained,
                StudentID = gradeDTO.StudentID,
                Note = gradeDTO.Note
            };

            await _context.Grades.AddAsync(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(GradeDTO gradeDTO)
        {
            var grade = await _context.Grades.FirstOrDefaultAsync(g => g.GradeID == gradeDTO.GradeID);
            if (grade == null) throw new KeyNotFoundException("Grade not found.");

            grade.EnrollmentID = gradeDTO.EnrollmentID;
            grade.ExamID = gradeDTO.ExamID;
            grade.MarksObtained = gradeDTO.MarksObtained;
            grade.StudentID = gradeDTO.StudentID;
            grade.Note = gradeDTO.Note;

            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<bool> DeleteGradeAsync(int gradeId)
        {
            var grade = await _context.Grades.FirstOrDefaultAsync(g => g.GradeID == gradeId);
            if (grade == null) return false;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetAverageGrade(int enrollmentId)
        {
            var grades = await _context.Grades
                .Where(g => g.EnrollmentID == enrollmentId)
                .Select(g => g.MarksObtained)
                .ToListAsync();

            return grades.Count > 0 ? grades.Average() : 0;
        }

        public async Task<IEnumerable<Grade>> GetGradesByStudentId(int studentId, DateTime startDate, DateTime endDate)
        {
            return await (
                from grade in _context.Grades
                join enrollment in _context.Enrollments
                on grade.EnrollmentID equals enrollment.EnrollmentID
                where enrollment.StudentID == studentId &&
                      enrollment.EnrollmentDate >= startDate &&
                      enrollment.EnrollmentDate <= endDate
                select new Grade
                {
                    GradeID = grade.GradeID,
                    EnrollmentID = grade.EnrollmentID,
                    ExamID = grade.ExamID,
                    MarksObtained = grade.MarksObtained,
                    Note = grade.Note,
                    Exam = new Exam
                    {
                        ExamID = grade.Exam.ExamID,
                        ClassID = grade.Exam.ClassID,
                        ExamDate = grade.Exam.ExamDate,
                        ExamType = grade.Exam.ExamType,
                        TotalMarks = grade.Exam.TotalMarks
                    }
                }
            ).ToListAsync();
        }

        public async Task<StudentGradesResult> GetStudentGradesAsync(int studentId)
        {
            var student = await _context.Students
                .Where(s => s.StudentID == studentId)
                .Select(s => new
                {
                    s.StudentID,
                    FullName = $"{s.FirstName} {s.LastName}"
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return null;

            var grades = await _context.Grades
                .Where(g => g.StudentID == studentId)
                .Join(
                    _context.Exams,
                    grade => grade.ExamID,
                    exam => exam.ExamID,
                    (grade, exam) => new
                    {
                        exam.ClassID,
                        exam.ExamType,
                        grade.MarksObtained,
                        exam.TotalMarks
                    })
                .ToListAsync(); 

            var groupedGrades = grades
                .GroupBy(g => g.ClassID)
                .Select(g => new ClassGradeResult
                {
                    ClassID = g.Key,
                    GradesByExamType = g
                        .GroupBy(e => e.ExamType)
                        .ToDictionary(
                            e => e.Key,
                            e => new ExamTypeResult
                            {
                                TotalMarks = e.Sum(x => x.TotalMarks),
                                MarksObtained = e.Sum(x => x.MarksObtained)
                            }
                        ),
                    AverageMarks = g.Average(x => x.MarksObtained)
                })
                .ToList();

            return new StudentGradesResult
            {
                FullName = student.FullName,
                Grades = groupedGrades
            };
        }


        //public async Task<List<GradeSummaryDto>> GetExamResultsByClassId(int classId)
        //{
        //    return await _context.Set<Grade>()
        //        .Where(g => g.Exam.ClassID == classId)
        //        .GroupBy(g => g.StudentID)
        //        .Select(g => new GradeSummaryDto
        //        {
        //            StudentID = g.Key,
        //            Grades = g.Select(x => new GradeDetailsDto
        //            {
        //                GradeID = x.GradeID,
        //                ExamID = x.ExamID,
        //                ExamType = x.Exam.ExamType,
        //                ExamDate = x.Exam.ExamDate,
        //                TotalMarks = x.Exam.TotalMarks,
        //                MarksObtained = x.MarksObtained,
        //            }).ToList(),
        //            AverageMarks = g.Average(x => x.MarksObtained)
        //        })
        //        .OrderByDescending(x => x.AverageMarks)
        //        .ToListAsync();
        //}


        public async Task<List<GradeSummaryDto>> GetExamResultsByClassId(int classId)
        {
            var studentsInClass = await _context.Set<Student>()
                .Where(s => s.Enrollments.Any(e => e.ClassID == classId))
                .Select(s => new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName
                })
                .ToListAsync();

            var examsInClass = await _context.Set<Exam>()
                .Where(e => e.ClassID == classId)
                .ToListAsync();

            var gradesInClass = await _context.Set<Grade>()
                .Where(g => examsInClass.Select(e => e.ExamID).Contains(g.ExamID))
                .ToListAsync();

            var result = studentsInClass.Select(student => new GradeSummaryDto
            {
                StudentID = student.StudentID,
                Grades = examsInClass.GroupJoin(
                    gradesInClass.Where(g => g.StudentID == student.StudentID),
                    exam => exam.ExamID,
                    grade => grade.ExamID,
                    (exam, grades) => grades.Select(grade => new GradeDetailsDto
                    {
                        GradeID = grade.GradeID,
                        ExamID = grade.ExamID,
                        ExamType = exam.ExamType,
                        ExamDate = exam.ExamDate,
                        TotalMarks = exam.TotalMarks,
                        MarksObtained = grade.MarksObtained,
                        Note = grade.Note
                    })
                    .DefaultIfEmpty(new GradeDetailsDto
                    {
                        ExamID = exam.ExamID,
                        ExamType = exam.ExamType,
                        ExamDate = exam.ExamDate,
                        TotalMarks = exam.TotalMarks,
                        MarksObtained = 0, // N/A cho sinh viên chưa có điểm
                        Note = "Chưa có điểm"
                    })
                    .FirstOrDefault() // Mỗi kỳ thi chỉ có 1 điểm
                ).ToList(),
                AverageMarks = gradesInClass.Any(g => g.StudentID == student.StudentID)
                    ? gradesInClass.Where(g => g.StudentID == student.StudentID).Average(g => g.MarksObtained)
                    : 0
            }).ToList();

            return result;
        }

        public async Task<bool> DeleteGradesByStudentAndClassAsync(int studentId, int classId)
        {
            try
            {
                var examsInClass = await _context.Set<Exam>()
                    .Where(e => e.ClassID == classId)
                    .Select(e => e.ExamID)
                    .ToListAsync();

                var gradesToDelete = await _context.Set<Grade>()
                    .Where(g => g.StudentID == studentId && examsInClass.Contains(g.ExamID))
                    .ToListAsync();

                if (!gradesToDelete.Any())
                    return false; // Không có điểm nào để xóa

                _context.Set<Grade>().RemoveRange(gradesToDelete);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa điểm.", ex);
            }
        }

        public async Task<IEnumerable<Class>> GetRetakeClassesAsync(int studentId)
        {
            var retakeClasses = await (
                from enrollment in _context.Enrollments
                join @class in _context.Classes on enrollment.ClassID equals @class.ClassID
                where enrollment.StudentID == studentId
                let averageGrade = _context.Grades
                    .Where(g => g.EnrollmentID == enrollment.EnrollmentID)
                    .Average(g => (decimal?)g.MarksObtained) ?? 0
                where averageGrade < 50 
                select @class
            ).ToListAsync();

            return retakeClasses;
        }

        public async Task<IEnumerable<StudentRetakeInfo>> GetStudentsRetakingClassAsync(int classId)
        {
            var studentsRetaking = await (
                from enrollment in _context.Enrollments
                join student in _context.Students on enrollment.StudentID equals student.StudentID
                where enrollment.ClassID == classId
                let averageGrade = _context.Grades
                    .Where(g => g.EnrollmentID == enrollment.EnrollmentID)
                    .Average(g => (decimal?)g.MarksObtained) ?? 0
                where averageGrade < 50 
                select new StudentRetakeInfo
                {
                    StudentID = student.StudentID,
                    FullName = $"{student.FirstName} {student.LastName}",
                    AverageGrade = averageGrade
                }
            ).ToListAsync();

            return studentsRetaking;
        }



        //public async Task<List<GradeSummaryDto>> GetExamResultsByClassId(int classId)
        //{
        //    var studentsInClass = await _context.Set<Student>()
        //        .Where(s => s.Enrollments.Any(e => e.ClassID == classId))
        //        .Select(s => new
        //        {
        //            s.StudentID,
        //            s.FirstName,
        //            s.LastName
        //        })
        //        .ToListAsync();

        //    var gradesInClass = await _context.Set<Grade>()
        //        .Where(g => g.Exam.ClassID == classId)
        //        .ToListAsync();

        //    var result = studentsInClass
        //        .GroupJoin(
        //            gradesInClass,
        //            student => student.StudentID,
        //            grade => grade.StudentID,
        //            (student, grades) => new GradeSummaryDto
        //            {
        //                StudentID = student.StudentID,
        //                Grades = grades.Select(g => new GradeDetailsDto
        //                {
        //                    GradeID = g.GradeID,
        //                    ExamID = g.ExamID,
        //                    ExamType = g.Exam?.ExamType ?? "N/A", // Gán giá trị mặc định nếu Exam null
        //                    ExamDate = g.Exam?.ExamDate,
        //                    TotalMarks = g.Exam?.TotalMarks ?? 0, // Gán giá trị mặc định nếu Exam null
        //                    MarksObtained = g.MarksObtained
        //                }).DefaultIfEmpty(new GradeDetailsDto
        //                {
        //                    MarksObtained = 0 // N/A cho các sinh viên chưa có điểm
        //                }).ToList(),
        //                AverageMarks = grades.Any() ? grades.Average(g => g.MarksObtained) : 0 // null nếu không có điểm
        //            }
        //        )
        //        .OrderByDescending(x => x.AverageMarks) // Xếp theo điểm trung bình
        //        .ToList();

        //    return result;
        //}


    }



}
