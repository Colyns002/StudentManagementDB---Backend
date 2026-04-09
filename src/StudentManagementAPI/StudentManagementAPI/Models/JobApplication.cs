using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobPostId { get; set; }
        public JobPost? Job { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string CoverLetter { get; set; } = string.Empty;
        
        [Required]
        public string ResumeLink { get; set; } = string.Empty;

        // Status: Pending, Accepted, Rejected
        public string Status { get; set; } = "Pending";
    }
}