using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VTYS.Models.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
                            IsApproved = true
                        };
                        _context.SelectedCourses.Add(selectedCourse);
                    }
                    await _context.SaveChangesAsync();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, student.Fullname),
                    new Claim(ClaimTypes.Email, student.EMail)
                };


                return RedirectToAction("Details", new { id = student.StudentId });
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            return View();
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await Task.Run(() => { });
            return RedirectToAction("Login", "Student");
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Instructor)
                .Include(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            return View(student);
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

        // GET: Student/UpdateInfo/0
        [HttpGet("UpdateInfo/{id}")]
        public async Task<IActionResult> UpdateInfo(int id)
        {
            try
            {
                if (id < 0)
                {
                    return BadRequest("Geçersiz ID.");
                }

                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return NotFound("Öğrenci bulunamadı.");
                }

                // Mevcut öğrenci modelini kullan
                return View(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GET UpdateInfo: {ex.Message}");
                return StatusCode(500, "Sunucu hatası. Lütfen tekrar deneyin.");
            }
        }

        // Post: Student/UpdateInfo/0
        [HttpPost("UpdateInfo/{id}")]
        public async Task<IActionResult> UpdateInfo(int id, Student student)
        {
            ModelState.Clear();
                if (!ModelState.IsValid)
                {
                    return View(student);
                }
                if (id != student.StudentId)
                {
                    return BadRequest("Geçersiz ID.");
                }
                var existingStudent = await _context.Students.FindAsync(id);
                if (existingStudent == null)
                {
                    return NotFound("Öğrenci bulunamadı.");
                }

                if (string.IsNullOrWhiteSpace(student.EMail) || string.IsNullOrWhiteSpace(student.Password))
                {
                    ModelState.AddModelError("", "E-posta ve şifre boş olamaz.");
                    return View(student);
                }

                existingStudent.EMail = student.EMail;
                existingStudent.Password = student.Password;

                _context.Entry(existingStudent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi!";
                return RedirectToAction("Details", new{id = student.StudentId});
        }
        
        [HttpGet("SelectAdjectiveCourse/{id}")]
        public async Task<IActionResult> SelectAdjectiveCourse(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            var AdjectiveCourses = await _context.Courses
                .Where(c => !c.IsMandatory && c.Class == student.Class)
                .ToListAsync();

            ViewBag.Student = student;

            return View(AdjectiveCourses);
        }

        [HttpPost("SelectAdjectiveCourse/{id}")]
        public async Task<IActionResult> SelectAdjectiveCourse(int id, int[] selectedCourses)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            foreach (var courseId in selectedCourses)
            {
                var AdjectiveCourses = await _context.Courses.FindAsync(courseId);
                if (AdjectiveCourses == null)
                {
                    return NotFound($"Kurs bulunamadı: {courseId}");
                }

                if (AdjectiveCourses.InstructorId == null)
                {
                    return BadRequest($"Kursun bir eğitmeni yok: {courseId}");
                }

                var selectedCourse = new SelectedCourse
                {
                    StudentId = student.StudentId,
                    CourseId = courseId,
                    InstructorId = AdjectiveCourses.InstructorId.Value,
                    IsApproved = false
                };
                _context.SelectedCourses.Add(selectedCourse);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Seçmeli dersler başarıyla seçildi!";
            return RedirectToAction("Details", new { id = student.StudentId });
        }
    }
}