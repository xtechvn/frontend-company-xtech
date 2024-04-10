using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;
using XTECH_FRONTEND.Services;

namespace XTECH_FRONTEND.Controllers
{
    public class NewsController : Controller
    {
        private readonly IConfiguration _configuration;
        public NewsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            ApiService apiService = new ApiService(_configuration);
            var requestObj =new GetListByCategoryIdRequest();
            requestObj.page = 1;
            requestObj.size = 10;
            requestObj.category_id = 2;
            var result = await apiService.GetNewsCategory(requestObj);
            if (result != null && result.status == 0)
            {
                ViewBag.data = result.data;
            }
            return View();
        }
        public async Task<IActionResult> Detail(long id)
        {
            ApiService apiService = new ApiService(_configuration);
            var result = await apiService.GetNewsDetail(id);
            if (result.status == 0)
            {
                ViewBag.data = result.data;
                ViewBag.id = id;
            }
            return View();

        }
        public async Task<IActionResult> GetlistNews(GetListByCategoryIdRequest requestObj)
        {
            ApiService apiService = new ApiService(_configuration);
  
            var result =await apiService.GetNewsCategory(requestObj);
           
            return Ok(result);

        }
       
    }
}
