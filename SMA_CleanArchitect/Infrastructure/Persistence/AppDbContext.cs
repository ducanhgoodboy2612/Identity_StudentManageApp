using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {

        public DbSet<Department> Departments_n { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TuitionFee> TuitionFees { get; set; }
        public DbSet<Test> Test { get; set; }

        public DbSet<User> Users { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.Property(e => e.EmailConfirmed)
                      .HasConversion<int>() // Chuyển bool -> int
                      .HasColumnType("NUMBER(1,0)");

                entity.Property(e => e.PhoneNumberConfirmed)
                      .HasConversion<int>()
                      .HasColumnType("NUMBER(1,0)");

                entity.Property(e => e.TwoFactorEnabled)
                      .HasConversion<int>()
                      .HasColumnType("NUMBER(1,0)");

                entity.Property(e => e.LockoutEnabled)
                      .HasConversion<int>()
                      .HasColumnType("NUMBER(1,0)");
            });

            // add index
            modelBuilder.Entity<Student>()
                .HasIndex(s => new { s.FirstName, s.LastName, s.Phone })
                .HasDatabaseName("idx_student_search");

            modelBuilder.HasSequence<int>("Student_Sequence")
                .IncrementsBy(100);

            modelBuilder.Entity<Student>()
                        .Property(s => s.StudentID)
                        .HasDefaultValueSql("Student_Sequence.NEXTVAL");

            modelBuilder.HasSequence<int>("Class_Sequence")
                .StartsAt(100)
                .IncrementsBy(1);

            modelBuilder.Entity<Class>()
                        .Property(c => c.ClassID)
                        .HasDefaultValueSql("Class_Sequence.NEXTVAL");

            modelBuilder.HasSequence<int>("Grade_Sequence")
            .StartsAt(100)  // Bắt đầu từ 100
            .IncrementsBy(1);  // Tăng 1 mỗi lần

            modelBuilder.Entity<Grade>()
                        .Property(g => g.GradeID)
                        .HasDefaultValueSql("Grade_Sequence.NEXTVAL");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseOracle(DBConnect.GetConnectionString());
        //    }
        //}
    }
}
