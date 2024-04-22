using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.Models.VPS;
using XTECH_FRONTEND.Services;
using XTECH_FRONTEND.Utilities;

namespace XTECH_FRONTEND.Controllers
{
    public class FAQController : Controller
    {

        private readonly IConfiguration _configuration;
        public FAQController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
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

        public async Task<IActionResult> GetPriceGalaxy(VPSmodel data)
        {
            try
            {
                ApiService apiService = new ApiService(_configuration);

                var result =await apiService.GetPriceGalaxy(data);
                
               
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistNews - NewsController: " + ex);
                return null;
            }
        }
    }
}
