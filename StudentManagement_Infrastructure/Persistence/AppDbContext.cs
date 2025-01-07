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

            // add index
            modelBuilder.Entity<Student>()
                .HasIndex(s => new { s.FirstName, s.LastName, s.Phone })
                .HasDatabaseName("idx_student_search");
            modelBuilder.Entity<Grade>()
                .HasIndex(g => g.StudentID)
                .HasDatabaseName("IX_Grade_StudentID");

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.ClassID)
                .HasDatabaseName("IX_Class_ClassID");

            modelBuilder.Entity<Schedule>()
                .HasIndex(c => c.ClassID)
                .HasDatabaseName("IX_Schedule_ClassID");

            modelBuilder.Entity<TuitionFee>()
                .HasIndex(c => c.StudentID)
                .HasDatabaseName("IX_TuitionFee_StudentID");

            // add sequence

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
            .StartsAt(100)
            .IncrementsBy(1);

            modelBuilder.Entity<Grade>()
                        .Property(g => g.GradeID)
                        .HasDefaultValueSql("Grade_Sequence.NEXTVAL");

            // Unique constraint 
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique()
                .HasDatabaseName("IX_Student_Email");

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Phone)
                .IsUnique()
                .HasDatabaseName("IX_Student_Phone");

            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.Email)
                .IsUnique()
                .HasDatabaseName("IX_Lecturer_Email");

            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.Phone)
                .IsUnique()
                .HasDatabaseName("IX_Lecturer_Phone");

            // Foreign key constraint for StudentID in Enrollment (References Student table)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student) // Specifies the navigation property in Enrollment
                .WithMany() // Optional: if Student has a collection of Enrollments
                .HasForeignKey(e => e.StudentID) // Specifies the column for foreign key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior

            // Foreign key constraint for ClassID in Enrollment (References Class table)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Class) // Specifies the navigation property in Enrollment
                .WithMany() // Optional: if Class has a collection of Enrollments
                .HasForeignKey(e => e.ClassID) // Specifies the column for foreign key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Course) // Specifies the navigation property in Class
                .WithMany() // Optional: if Course has a collection of Classes
                .HasForeignKey(c => c.CourseID) // Specifies the column for foreign key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior

            // Foreign key constraint for LecturerID in Class (References Lecturer table)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Lecturer) // Specifies the navigation property in Class
                .WithMany() // Optional: if Lecturer has a collection of Classes
                .HasForeignKey(c => c.LecturerID) // Specifies the column for foreign key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior

            //modelBuilder.Entity<Student>()
            //    .HasCheckConstraint("CK_Student_Gender", "Gender IN (0, 1)");

        }
    }
    
}
