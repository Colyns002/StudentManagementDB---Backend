namespace StudentManagementAPI.Models
{
    public class JobPostCourse
    {
        public int JobPostId { get; set; }
        public JobPost? JobPost { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        /// <summary>Optional reasoning or logic for the recommendation (e.g. "Core Requirement", "Highly Recommended").</summary>
        public string? RecommendationLevel { get; set; } = "Recommended";
    }
}
