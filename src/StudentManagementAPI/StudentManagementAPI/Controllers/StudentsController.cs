using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/students
        [Authorize(Roles = "Employer,Admin")] // Employers can see all to assist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/students/my-profile
        [Authorize(Roles = "Student")]
        [HttpGet("my-profile")]
        public async Task<ActionResult<Student>> GetMyProfile()
        {
            var userEmail = User.Identity?.Name; // In our JWT, Name is usually the email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null) return NotFound("Student profile not found.");
            return student;
        }

        // POST: api/students
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudents), new { id = student.StudentID }, student);
        }

        // PUT: api/students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.StudentID == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}