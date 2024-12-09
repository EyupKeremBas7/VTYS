using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VTYS.Models.Entity;

namespace VTYS.Controllers
{
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly VtysContext _context;

        public StudentController(VtysContext context)
        {
            _context = context;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var student = await _context.Students
                .Include(s => s.Instructor)
                .Include(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.EMail == email && s.Password == password);

            if (student != null)
            {
                var assignedCourses = await _context.SelectedCourses
                    .Where(sc => sc.StudentId == student.StudentId)
                    .ToListAsync();

                if (!assignedCourses.Any())
                {
                    var mandatoryCourses = await _context.Courses
                        .Where(c => c.IsMandatory && c.Class == student.Class)
                        .ToListAsync();

                    foreach (var course in mandatoryCourses)
                    {
                        var selectedCourse = new SelectedCourse
                        {
                            StudentId = student.StudentId,
                            CourseId = course.CourseId,
                            InstructorId = course.InstructorId,
                            IsApproved = true // Automatically approve mandatory courses
                        };

                        _context.SelectedCourses.Add(selectedCourse);
                    }

                    await _context.SaveChangesAsync();
                }

                return View("StudentDetails", student);
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            return View();
        }

        // GET: Student/getStudentList
        [HttpGet("getStudentList")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: Student/getById
        [HttpGet("getById")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            return student;
        }

        // PUT: Student/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest(new { Message = "Student ID mismatch." });
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound(new { Message = "Student not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Student
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: Student/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        // GET: Student/getByAdvisorId
        [HttpGet("getByAdvisorId")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByInstructorId(int InstructorId)
        {
            var students = await _context.Students
                                        .Where(s => s.InstructorId == InstructorId)
                                        .ToListAsync();

            if (!students.Any())
            {
                return NotFound(new { Message = $"{InstructorId} bu akademisyenin danışmanlık." });
            }

            return Ok(students);
        }
    }
}