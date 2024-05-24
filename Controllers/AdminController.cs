using Microsoft.AspNetCore.Mvc;

namespace Eats_Tech.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
