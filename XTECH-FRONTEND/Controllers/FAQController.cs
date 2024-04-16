using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.Services;

namespace XTECH_FRONTEND.Controllers
{
    public class FAQController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DetailDeginwebsite()
        {
            
            return View();

        }
        public async Task<IActionResult> DetailVPS()
        {

            return View();

        }
    }
}
