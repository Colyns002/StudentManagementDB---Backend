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
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ───────────────────────────────────────────────
        // GET: api/Courses          (Public – everyone can browse)
        // ───────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive)
                .Include(c => c.Instructor)
                .Include(c => c.JobPostCourses)
                    .ThenInclude(jpc => jpc.JobPost)
                .Select(c => new CourseResponseDto
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    Description = c.Description,
                    Duration = c.Duration,
                    Credits = c.Credits,
                    DeptID = c.DeptID,
                    Price = c.Price,
                    Syllabus = c.Syllabus,
                    Level = c.Level,
                    CreatedDate = c.CreatedDate,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor != null ? c.Instructor.UserName : null,
                    RecommendedJobs = c.JobPostCourses.Select(jpc => new RecommendedJobDto
                    {
                        JobId = jpc.JobPostId,
                        Title = jpc.JobPost != null ? jpc.JobPost.Title : "Unknown"
                    }).ToList()
                })
                .ToListAsync();

            return Ok(courses);
        }

        // ───────────────────────────────────────────────
        // GET: api/Courses/{id}     (Public – course details)
        // ───────────────────────────────────────────────
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseResponseDto>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.JobPostCourses)
                    .ThenInclude(jpc => jpc.JobPost)
                .FirstOrDefaultAsync(c => c.CourseID == id);

            if (course == null || !course.IsActive)
                return NotFound("Course not found.");

            var dto = new CourseResponseDto
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Description = course.Description,
                Duration = course.Duration,
                Credits = course.Credits,
                DeptID = course.DeptID,
                Price = course.Price,
                Syllabus = course.Syllabus,
                Level = course.Level,
                CreatedDate = course.CreatedDate,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor?.UserName,
                RecommendedJobs = course.JobPostCourses.Select(jpc => new RecommendedJobDto
                {
                    JobId = jpc.JobPostId,
                    Title = jpc.JobPost != null ? jpc.JobPost.Title : "Unknown"
                }).ToList()
            };

            return Ok(dto);
        }

        // ───────────────────────────────────────────────
        // POST: api/Courses         (Employer / Admin only)
        // ───────────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse(CreateCourseDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Duration = dto.Duration,
                Credits = dto.Credits,
                DeptID = dto.DeptID,
                Price = dto.Price,
                Syllabus = dto.Syllabus,
                Level = dto.Level,
                InstructorId = userId!,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var response = new CourseResponseDto
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Description = course.Description,
                Duration = course.Duration,
                Credits = course.Credits,
                DeptID = course.DeptID,
                Price = course.Price,
                Syllabus = course.Syllabus,
                Level = course.Level,
                CreatedDate = course.CreatedDate,
                InstructorId = course.InstructorId,
                InstructorName = null // not loaded yet
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseID }, response);
        }

        // ───────────────────────────────────────────────
        // PUT: api/Courses/{id}     (Employer / Admin only)
        // ───────────────────────────────────────────────
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto dto)
        {
            var existing = await _context.Courses.FindAsync(id);
            if (existing == null || !existing.IsActive)
                return NotFound("Course not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Only the creator or an Admin can update
            if (!isAdmin && existing.InstructorId != userId)
                return Forbid("You can only update your own courses.");

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Duration = dto.Duration;
            existing.Credits = dto.Credits;
            existing.DeptID = dto.DeptID;
            existing.Price = dto.Price;
            existing.Syllabus = dto.Syllabus;
            existing.Level = dto.Level;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ───────────────────────────────────────────────
        // DELETE: api/Courses/{id}  (Employer / Admin – soft delete)
        // ───────────────────────────────────────────────
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var existing = await _context.Courses.FindAsync(id);
            if (existing == null || !existing.IsActive)
                return NotFound("Course not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existing.InstructorId != userId)
                return Forbid("You can only delete your own courses.");

            // Soft delete – keeps historical data intact
            existing.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}