namespace StudentManagementAPI.DTOs
{
    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
        public string EmployerId { get; set; } = string.Empty;
        public string? EmployerName { get; set; }
        
        // List of courses recommended for this job
        public List<RecommendedCourseDto> RecommendedCourses { get; set; } = new List<RecommendedCourseDto>();
    }

    public class RecommendedCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }

    public class RecommendedJobDto
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
