using Microsoft.AspNetCore.Mvc;

namespace Eats_Tech.Controllers
{
    public class CocinaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
