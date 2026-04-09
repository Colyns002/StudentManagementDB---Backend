using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.Models
{
    public class JobPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // The ID of the User (Staff/Admin) who posted the job
        public string EmployerId { get; set; } = string.Empty;
        public ApplicationUser? Employer { get; set; }

        // Navigation property for applications
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

        // Navigation property for course recommendations (Many-to-Many)
        public ICollection<JobPostCourse> JobPostCourses { get; set; } = new List<JobPostCourse>();
    }
}