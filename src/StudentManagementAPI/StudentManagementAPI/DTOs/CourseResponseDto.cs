namespace StudentManagementAPI.DTOs
{
    /// <summary>DTO returned to clients when fetching course details.</summary>
    public class CourseResponseDto
    {
        public int CourseID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int DeptID { get; set; }
        public decimal Price { get; set; }
        public string? Syllabus { get; set; }
        public string Level { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public string? InstructorName { get; set; }
        
        // List of jobs where this course is recommended
        public List<RecommendedJobDto> RecommendedJobs { get; set; } = new List<RecommendedJobDto>();
    }
}
