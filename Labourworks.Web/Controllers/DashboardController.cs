using Microsoft.AspNetCore.Mvc;

namespace Labourworks.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
