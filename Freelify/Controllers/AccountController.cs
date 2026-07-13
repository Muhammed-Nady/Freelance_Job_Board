using Microsoft.AspNetCore.Mvc;

namespace Freelify.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
