using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VTYS.Models.Entity;
using System.Security.Claims;

namespace VTYS.Controllers
{
    [Route("[controller]")]
    public class InstructorController : Controller
    {
        private readonly VtysContext _context;

        public InstructorController(VtysContext context)
        {
            _context = context;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email,string password)
        {
            var instructor = await _context.Instructors
                .Include(i => i.Students)
                .ThenInclude(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(i => i.EMail == email && i.Password == password);

            if (instructor != null)
            {
                return View("Details", instructor);
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            return View(instructor);
        }
        
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.Students)
                .ThenInclude(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.InstructorId == id);

            if (instructor == null)
            {
                return NotFound(new { Message = "Instructor not found." });
            }

            ViewBag.InstructorId = id;
            return View(instructor);
        }

        // GET: Instructor/getInstructorList
        [HttpGet("getInstructorList")]
        public async Task<ActionResult<IEnumerable<Instructor>>> GetInstructors()
        {
            return await _context.Instructors.ToListAsync();
        }

        // GET: Instructor/getById
        [HttpGet("getById")]
        public async Task<ActionResult<Instructor>> GetInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return NotFound(new { Message = "Instructor not found." });
            }

            return instructor;
        }

        // PUT: Instructor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor(int id, Instructor instructor)
        {
            if (id != instructor.InstructorId)
            {
                return BadRequest(new { Message = "Instructor ID mismatch." });
            }

            _context.Entry(instructor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstructorExists(id))
                {
                    return NotFound(new { Message = "Instructor not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Instructor
        [HttpPost]
        public async Task<ActionResult<Instructor>> CreateInstructor(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInstructor", new { id = instructor.InstructorId }, instructor);
        }

        // DELETE: Instructor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound(new { Message = "Instructor not found." });
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.InstructorId == id);
        }
        // GET: Instructor/UpdateInfo/0
        [HttpGet("UpdateInfo/{id}")]
        public async Task<IActionResult> UpdateInfo(int id)
        {
            try
            {
                if (id < 0)
                {
                    return BadRequest("Geçersiz ID.");
                }

                var instructor = await _context.Instructors.FindAsync(id);
                if (instructor == null)
                {
                    return NotFound("Akademisyen bulunamadı.");
                }

                return View(instructor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GET UpdateInfo: {ex.Message}");
                return StatusCode(500, "Sunucu hatası. Lütfen tekrar deneyin.");
            }
        }
        // Post: Instructor/UpdateInfo/0
        [HttpPost("UpdateInfo/{id}")]
        public async Task<IActionResult> UpdateInfo(int id, Instructor instructor)
        {
            ModelState.Clear();
                if (!ModelState.IsValid)
                {
                    return View(instructor);
                }
                if (id != instructor.InstructorId)
                {
                    return BadRequest("Geçersiz ID.");
                }
                var existingInstructor = await _context.Instructors.FindAsync(id);
                if (existingInstructor == null)
                {
                    return NotFound("Akademisyen bulunamadı.");
                }

                if (string.IsNullOrWhiteSpace(instructor.EMail) || string.IsNullOrWhiteSpace(instructor.Password))
                {
                    ModelState.AddModelError("", "E-posta ve şifre boş olamaz.");
                    return View(instructor);
                }

                existingInstructor.EMail = instructor.EMail;
                existingInstructor.Password = instructor.Password;

                _context.Entry(existingInstructor).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi!";
                return RedirectToAction("Details", new{id = instructor.InstructorId});
        }
        
        [HttpGet("AdjectiveCourse/{id}")]
        public async Task<IActionResult> AdjectiveCourse(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound("Instructor not found.");
            }

            var selectedCourses = await _context.SelectedCourses
                .Include(sc => sc.Course)
                .Include(sc => sc.Student)
                .Where(sc => sc.InstructorId == id && !sc.IsApproved)
                .ToListAsync();

            ViewBag.Instructor = instructor;

            return View(selectedCourses);
        }

        [HttpPost("ApproveCourse/{id}")]
        public async Task<IActionResult> ApproveCourse(int id)
        {
            var selectedCourse = await _context.SelectedCourses.FindAsync(id);
            if (selectedCourse == null)
            {
                return NotFound("Seçmeli ders bulunamadı.");
            }

            selectedCourse.IsApproved = true;
            _context.Entry(selectedCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ders başarıyla onaylandı!";
            return Ok();
        }

        [HttpPost("RejectCourse/{id}")]
        public async Task<IActionResult> RejectCourse(int id)
        {
            var selectedCourse = await _context.SelectedCourses.FindAsync(id);
            if (selectedCourse == null)
            {
                return NotFound("Seçmeli ders bulunamadı.");
            }

            _context.SelectedCourses.Remove(selectedCourse);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ders başarıyla reddedildi!";
            return View();
        }
    }
}