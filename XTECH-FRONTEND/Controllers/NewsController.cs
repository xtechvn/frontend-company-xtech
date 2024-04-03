using Microsoft.AspNetCore.Mvc;

namespace XTECH_FRONTEND.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(long id)
        {
            return View();

        }
    }
}
