using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VTYS.Models.Entity;
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

        //Get Student/SelectAdjectiveCourse/0
        [HttpGet("SelectAdjectiveCourse/{id}")]
        public async Task<IActionResult> SelectAdjectiveCourse(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            var courses = await _context.Courses
                .Where(c => !c.IsMandatory && c.Class == student.Class)
                .ToListAsync();

            var selectedCourse = await _context.SelectedCourses
                .Where(sc => sc.StudentId == student.StudentId)
                .ToListAsync();

            ViewBag.Student = student;

            return View(courses);
        }

        // Post: Student/SelectAdjectiveCourse/0
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
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    return NotFound($"Kurs bulunamadı: {courseId}");
                }

                if (course.InstructorId == null)
                {
                    return BadRequest($"Kursun bir eğitmeni yok: {courseId}");
                }

                var selectedCourse = new SelectedCourse
                {
                    StudentId = student.StudentId,
                    CourseId = courseId,
                    InstructorId = course.InstructorId.Value,
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