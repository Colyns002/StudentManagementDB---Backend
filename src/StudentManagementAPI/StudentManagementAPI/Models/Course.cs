using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementAPI.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Duration of the course (e.g., "6 Weeks", "3 Months").</summary>
        [Required]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;

        public int Credits { get; set; }

        /// <summary>Foreign key to the Department this course belongs to.</summary>
        public int DeptID { get; set; }

        /// <summary>The UserId of the Employer/Admin who created the course. Nullable for legacy/seed data.</summary>
        public string? InstructorId { get; set; }

        /// <summary>Navigation property to the instructor (ApplicationUser).</summary>
        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }

        /// <summary>Price of the course. 0 means free.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        /// <summary>Course syllabus or requirements, stored as plain text or markdown.</summary>
        [StringLength(5000)]
        public string? Syllabus { get; set; }

        /// <summary>Skill level: Beginner, Intermediate, Advanced.</summary>
        [StringLength(50)]
        public string Level { get; set; } = "Beginner";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property for job recommendations (Many-to-Many)
        public ICollection<JobPostCourse> JobPostCourses { get; set; } = new List<JobPostCourse>();
    }
}