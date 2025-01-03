using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StudentManagement_Domain.Entities;
namespace StudentManagement_Infrastructure.Persistence
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
                entity.ToTable("AspNetUsers", "STUDENT_INTERNPROJECT");

                // Map properties to the correct Oracle column types
                entity.Property(u => u.Id)
                    .HasColumnName("Id")
                    .HasColumnType("NVARCHAR2(450)")
                    .IsRequired();

                entity.Property(u => u.UserName)
                    .HasColumnName("UserName")
                    .HasColumnType("NVARCHAR2(256)");

                entity.Property(u => u.NormalizedUserName)
                    .HasColumnName("NormalizedUserName")
                    .HasColumnType("NVARCHAR2(256)");

                entity.Property(u => u.Email)
                    .HasColumnName("Email")
                    .HasColumnType("NVARCHAR2(256)");

                entity.Property(u => u.NormalizedEmail)
                    .HasColumnName("NormalizedEmail")
                    .HasColumnType("NVARCHAR2(256)");

                entity.Property(u => u.EmailConfirmed)
                .HasColumnName("EmailConfirmed")
                .HasColumnType("NUMBER(1)")
                .IsRequired();

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("PasswordHash")
                    .HasColumnType("NVARCHAR2(2000)");

                entity.Property(u => u.SecurityStamp)
                .HasColumnName("SecurityStamp")
                .HasColumnType("NVARCHAR2(2000)");

                entity.Property(u => u.ConcurrencyStamp)
                    .HasColumnName("ConcurrencyStamp")
                    .HasColumnType("NVARCHAR2(2000)");

                entity.Property(u => u.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasColumnType("NVARCHAR2(2000)");

                entity.Property(u => u.PhoneNumberConfirmed)
                    .HasColumnName("PhoneNumberConfirmed")
                    .HasColumnType("NUMBER(1)")
                    .IsRequired();

                entity.Property(u => u.TwoFactorEnabled)
                    .HasColumnName("TwoFactorEnabled")
                    .HasColumnType("NUMBER(1)")
                    .IsRequired();

                entity.Property(u => u.LockoutEnd)
                    .HasColumnName("LockoutEnd")
                    .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                entity.Property(u => u.LockoutEnabled)
                    .HasColumnName("LockoutEnabled")
                    .HasColumnType("NUMBER(1)")
                    .IsRequired();

                entity.Property(u => u.AccessFailedCount)
                    .HasColumnName("AccessFailedCount")
                    .HasColumnType("NUMBER(10)")
                    .IsRequired();
            });
        }
    }
}
