using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
