using Microsoft.AspNetCore.Mvc;

namespace FFMonitorWeb.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
