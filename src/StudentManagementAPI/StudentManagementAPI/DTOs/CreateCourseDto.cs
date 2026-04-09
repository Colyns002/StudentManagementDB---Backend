using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.DTOs
{
    /// <summary>DTO used by Employers/Admins when creating a new course.</summary>
    public class CreateCourseDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;

        public int Credits { get; set; }

        public int DeptID { get; set; }

        public decimal Price { get; set; } = 0;

        [StringLength(5000)]
        public string? Syllabus { get; set; }

        [StringLength(50)]
        public string Level { get; set; } = "Beginner";
    }
}
