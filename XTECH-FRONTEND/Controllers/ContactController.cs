using Microsoft.AspNetCore.Mvc;

namespace XTECH_FRONTEND.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
