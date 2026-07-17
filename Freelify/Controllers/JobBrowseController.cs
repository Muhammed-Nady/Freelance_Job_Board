using Freelify.Models.ViewModels.Job;
using Freelify.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Freelify.Controllers
{
    public class JobBrowseController : Controller
    {
        private readonly JobService _jobService;

        public JobBrowseController(JobService  jobService)
        {
            _jobService = jobService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(JobBrowseViewModel model)
        {
            var result = await _jobService.GetAllJobsWithFilters(model);
            
            return View(result);
        }
    }
}
