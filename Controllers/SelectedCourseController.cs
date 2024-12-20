using Microsoft.AspNetCore.Mvc;
using VTYS.Models.Entity;

namespace VTYS.Controllers
{
    [Route("[controller]")]
    public class SelectedCourseController : Controller
    {
        private readonly ILogger<SelectedCourseController> _logger;

        public SelectedCourseController(ILogger<SelectedCourseController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View("Error");
        }
        
    }
    
}