namespace StudentManagementAPI.DTOs
{
    public class JobApplicationResponseDto
    {
        public int Id { get; set; }
        public int JobPostId { get; set; }
        public string? JobTitle { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public string ResumeLink { get; set; } = string.Empty;
    }
}
