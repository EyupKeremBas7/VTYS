using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VTYS.Models.Entity;

namespace VTYS.Controllers
{
    [Route("[controller]")]
    public class SelectAdjectiveCourseController : Controller
    {
        private readonly VtysContext _context;
        private readonly ILogger<SelectAdjectiveCourseController> _logger;

        public SelectAdjectiveCourseController(VtysContext context, ILogger<SelectAdjectiveCourseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("SelectCourse")]
        public async Task<IActionResult> SelectCourse()
        {
            var student = await _context.Students
                .Include(s => s.SelectedCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.EMail == User.Identity.Name);

            if (student == null)
            {
                return NotFound();
            }

            var availableCourses = await _context.Courses
                .Where(c => !c.IsMandatory && c.Class == student.Class)
                .ToListAsync();

            ViewBag.AvailableCourses = availableCourses;

            return View("StudentSelectAdjectiveCourse", student);
        }

        [HttpPost("SelectCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAdjectiveCourse(int courseId)
        {
            var student = await _context.Students
                .Include(s => s.SelectedCourses)
                .FirstOrDefaultAsync(s => s.EMail == User.Identity.Name);

            if (student == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(courseId);

            if (course == null || course.IsMandatory || course.Class != student.Class)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz ders seçimi.");
                return RedirectToAction("SelectCourse");
            }

            var selectedCourse = new SelectedCourse
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                InstructorId = course.InstructorId,
                IsApproved = false // Seçmeli dersler otomatik olarak onaylanmaz
            };

            _context.SelectedCourses.Add(selectedCourse);
            await _context.SaveChangesAsync();

            return RedirectToAction("SelectCourse");
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}