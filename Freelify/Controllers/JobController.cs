using Microsoft.AspNetCore.Mvc;

namespace Freelify.Controllers
{
    public class JobController : Controller
    {

        [HttpPost]
        public IActionResult Create()
        {
            return View();
        }
    }
}
