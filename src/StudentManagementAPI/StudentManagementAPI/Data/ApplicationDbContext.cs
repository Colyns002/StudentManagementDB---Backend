using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobPostCourse> JobPostCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Many-to-Many Join Table: JobPost <-> Course
            builder.Entity<JobPostCourse>()
                .HasKey(jpc => new { jpc.JobPostId, jpc.CourseId });

            builder.Entity<JobPostCourse>()
                .HasOne(jpc => jpc.JobPost)
                .WithMany(j => j.JobPostCourses)
                .HasForeignKey(jpc => jpc.JobPostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobPostCourse>()
                .HasOne(jpc => jpc.Course)
                .WithMany(c => c.JobPostCourses)
                .HasForeignKey(jpc => jpc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed a Department
            builder.Entity<Department>().HasData(
                new Department
                {
                    DeptID = 1,
                    Name = "Computer Science"
                }
            );

            // Seed a Course
            builder.Entity<Course>().HasData(
                new Course
                {
                    CourseID = 1,
                    Title = "Backend Development",
                    Description = "Learn to build robust REST APIs with ASP.NET Core, Entity Framework, and SQL Server.",
                    Duration = "8 Weeks",
                    Credits = 4,
                    DeptID = 1,
                    Price = 0,
                    Level = "Intermediate",
                    CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );

            // Seed a Student
            builder.Entity<Student>().HasData(
                new Student
                {
                    StudentID = 1,
                    FirstName = "AgileBiz",
                    LastName = "Tester",
                    Email = "test.user@agilebiz.com",
                    DateOfBirth = new DateTime(2000, 1, 1), // Matches your DateOfBirth property
                    DeptID = 1 // Links the student to the CS department
                }
            );

            // Fix the "Multiple Cascade Paths" error for JobApplications
            builder.Entity<JobApplication>()
        .HasOne(ja => ja.Job)
        .WithMany(j => j.Applications)
        .HasForeignKey(ja => ja.JobPostId)
        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplication>()
                .HasOne(ja => ja.Student)
                .WithMany()
                .HasForeignKey(ja => ja.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course → Instructor (ApplicationUser) – Restrict to avoid cascade conflicts
            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany()
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
