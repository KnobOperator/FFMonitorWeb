using Microsoft.AspNetCore.Mvc;

namespace FFMonitorWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
