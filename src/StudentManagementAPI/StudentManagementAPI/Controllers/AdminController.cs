using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            var stats = new
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalJobs = await _context.JobPosts.CountAsync(),
                TotalApplications = await _context.JobApplications.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync()
            };

            return Ok(stats);
        }
    }
}
