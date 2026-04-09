using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;
using StudentManagementAPI.DTOs;
using System.Security.Claims;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires login for all actions
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobResponseDto>>> GetJobs()
        {
            var jobs = await _context.JobPosts
                .Include(j => j.Employer)
                .Include(j => j.JobPostCourses)
                    .ThenInclude(jpc => jpc.Course)
                .Where(j => j.IsActive)
                .Select(j => new JobResponseDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    PostedDate = j.PostedDate,
                    EmployerId = j.EmployerId,
                    EmployerName = j.Employer != null ? (j.Employer.UserName) : "Unknown",
                    RecommendedCourses = j.JobPostCourses.Select(jpc => new RecommendedCourseDto
                    {
                        CourseId = jpc.CourseId,
                        Title = jpc.Course != null ? jpc.Course.Title : "Unknown",
                        Level = jpc.Course != null ? jpc.Course.Level : "General"
                    }).ToList()
                })
                .ToListAsync();

            return Ok(jobs);
        }

        // GET: api/Jobs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobResponseDto>> GetJobPost(int id)
        {
            var j = await _context.JobPosts
                .Include(j => j.Employer)
                .Include(j => j.JobPostCourses)
                    .ThenInclude(jpc => jpc.Course)
                .FirstOrDefaultAsync(jo => jo.Id == id && jo.IsActive);

            if (j == null)
            {
                return NotFound();
            }

            var dto = new JobResponseDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                PostedDate = j.PostedDate,
                EmployerId = j.EmployerId,
                EmployerName = j.Employer != null ? (j.Employer.UserName) : "Unknown",
                RecommendedCourses = j.JobPostCourses.Select(jpc => new RecommendedCourseDto
                {
                    CourseId = jpc.CourseId,
                    Title = jpc.Course != null ? jpc.Course.Title : "Unknown",
                    Level = jpc.Course != null ? jpc.Course.Level : "General"
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/Jobs/{id}/LinkCourse/{courseId}
        [HttpPost("{id}/LinkCourse/{courseId}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> LinkCourse(int id, int courseId)
        {
            var job = await _context.JobPosts.FindAsync(id);
            var course = await _context.Courses.FindAsync(courseId);

            if (job == null || course == null)
                return NotFound("Job or Course not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && job.EmployerId != userId)
                return Forbid("You can only link courses to your own job posts.");

            // Check if already linked
            var existing = await _context.JobPostCourses
                .AnyAsync(jpc => jpc.JobPostId == id && jpc.CourseId == courseId);
            
            if (existing) return Ok("Already linked.");

            var link = new JobPostCourse { JobPostId = id, CourseId = courseId };
            _context.JobPostCourses.Add(link);
            await _context.SaveChangesAsync();

            return Ok("Course linked to job successfully.");
        }

        // POST: api/Jobs
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")] // Employers and Admins can post
        public async Task<ActionResult<JobPost>> PostJob(JobPost jobPost)
        {
            // Get the ID of the logged-in Employer
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            jobPost.EmployerId = userId;
            jobPost.PostedDate = DateTime.Now;
            jobPost.IsActive = true;

            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPost", new { id = jobPost.Id }, jobPost);
        }

        // POST: api/Jobs/{id}/Apply
        [HttpPost("{id}/Apply")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<JobApplication>> ApplyForJob(int id, ApplyJobDto applyJobDto)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null || !jobPost.IsActive)
                return NotFound("Job post not found or inactive.");

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized("Student ID not found.");

            // Check if already applied
            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.JobPostId == id && ja.StudentId == studentId);
            if (existingApplication != null)
                return BadRequest("You have already applied for this job.");

            var application = new JobApplication
            {
                JobPostId = id,
                StudentId = studentId,
                CoverLetter = applyJobDto.CoverLetter,
                ResumeLink = applyJobDto.ResumeLink,
                AppliedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(application);
        }

        // GET: api/Jobs/MyApplications
        [HttpGet("MyApplications")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<JobApplicationResponseDto>>> GetMyApplications()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applications = await _context.JobApplications
                .Include(ja => ja.Job)
                .Include(ja => ja.Student)
                .Where(ja => ja.StudentId == studentId)
                .Select(ja => new JobApplicationResponseDto
                {
                    Id = ja.Id,
                    JobPostId = ja.JobPostId,
                    JobTitle = ja.Job != null ? ja.Job.Title : null,
                    StudentName = ja.Student != null ? ja.Student.UserName : null,
                    StudentEmail = ja.Student != null ? ja.Student.Email : null,
                    AppliedDate = ja.AppliedDate,
                    Status = ja.Status,
                    CoverLetter = ja.CoverLetter,
                    ResumeLink = ja.ResumeLink
                })
                .ToListAsync();

            return Ok(applications);
        }

        // GET: api/Jobs/{id}/Applications
        [HttpGet("{id}/Applications")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<ActionResult<IEnumerable<JobApplicationResponseDto>>> GetJobApplications(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isEmployer = User.IsInRole("Employer");
            
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null)
                return NotFound("Job post not found.");
                
            if (isEmployer && jobPost.EmployerId != userId)
                return Forbid("You can only view applications for your own job posts.");

            var applications = await _context.JobApplications
                .Include(ja => ja.Student)
                .Where(ja => ja.JobPostId == id)
                .Select(ja => new JobApplicationResponseDto
                {
                    Id = ja.Id,
                    JobPostId = ja.JobPostId,
                    JobTitle = jobPost.Title,
                    StudentName = ja.Student != null ? ja.Student.UserName : null,
                    StudentEmail = ja.Student != null ? ja.Student.Email : null,
                    AppliedDate = ja.AppliedDate,
                    Status = ja.Status,
                    CoverLetter = ja.CoverLetter,
                    ResumeLink = ja.ResumeLink
                })
                .ToListAsync();

            return Ok(applications);
        }

        // PATCH: api/Jobs/Applications/{appId}/Status
        [HttpPatch("Applications/{appId}/Status")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> UpdateApplicationStatus(int appId, ApplicationStatusUpdateDto statusUpdate)
        {
            var application = await _context.JobApplications
                .Include(ja => ja.Job)
                .FirstOrDefaultAsync(ja => ja.Id == appId);

            if (application == null)
                return NotFound("Application not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isEmployer = User.IsInRole("Employer");

            if (isEmployer && application.Job != null && application.Job.EmployerId != userId)
                return Forbid("You can only update applications for your own job posts.");

            application.Status = statusUpdate.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // PUT: api/Jobs/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> PutJob(int id, JobPost jobPost)
        {
            if (id != jobPost.Id)
            {
                return BadRequest("Job ID mismatch.");
            }

            var existingJob = await _context.JobPosts.FindAsync(id);
            if (existingJob == null || !existingJob.IsActive)
            {
                return NotFound("Job not found.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existingJob.EmployerId != userId)
            {
                return Forbid("You can only edit your own job posts.");
            }

            // Update specific fields
            existingJob.Title = jobPost.Title;
            existingJob.Description = jobPost.Description;
            // Leave EmployerId, PostedDate, IsActive unchanged

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Jobs/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var existingJob = await _context.JobPosts.FindAsync(id);
            if (existingJob == null || !existingJob.IsActive)
            {
                return NotFound("Job not found.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existingJob.EmployerId != userId)
            {
                return Forbid("You can only delete your own job posts.");
            }

            // Soft delete
            existingJob.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}