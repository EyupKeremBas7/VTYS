using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VTYS.Models.Entity;

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
        public async Task<IActionResult> Login(string email, int password)
        {
            var instructor = await _context.Instructors
                .Include(i => i.Students)
                .ThenInclude(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(i => i.EMail == email && i.InstructorId == password);

            if (instructor != null)
            {
                return View("InstructorDetails", instructor);
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
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
    }
}