using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.Models.News;
using XTECH_FRONTEND.Models;
using XTECH_FRONTEND.Models.News.FindArticle;
using XTECH_FRONTEND.Models.News.GetCategory;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;
using XTECH_FRONTEND.Services;
using XTECH_FRONTEND.Utilities;

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
            try
            {
                ApiService apiService = new ApiService(_configuration);
                var requestObj = new GetListByCategoryIdRequest();
                requestObj.page = 1;
                requestObj.size = 10;
                requestObj.category_id = 1004;
                var result = await apiService.GetNewsByCategoryId(requestObj);
                if (result != null && result.status == 0)
                {
                    ViewBag.data = result.data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Index - NewsController: " + ex);
            }
            return View();
        }
        public async Task<IActionResult> Detail(long id)
        {
            try
            {
                ApiService apiService = new ApiService(_configuration);
                var result = await apiService.GetNewsDetail(id);

                if (result.status == 0)
                {
                    ViewBag.data = result.data;
                    ViewBag.id = id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - NewsController: " + ex);
            }
            return View();

        }
        public async Task<IActionResult> GetlistNews(GetListByCategoryIdRequest requestObj)
        {
            try
            {
                ApiService apiService = new ApiService(_configuration);

                var result = await apiService.GetNewsByCategoryId(requestObj);

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistNews - NewsController: " + ex);
                return null;
            }
        }
        public async Task<IActionResult> GetNewsCategory(long id)
        {
            try
            {

                ApiService apiService = new ApiService(_configuration);

                var result = await apiService.GetNewsCategory(id);
                if (result != null && result.categories.Count > 0)
                {
                    foreach (var item in result.categories)
                    {
                        var result2 = await apiService.GetNewsCategory(item.id);
                        if (result2 != null && result2.categories.Count > 0)
                        {
                            item.ListCategoryResponse = result2.categories;
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsCategory - NewsController: " + ex);

                return null;
            }
        }
        public async Task<IActionResult> GetFindArticle(FindArticleRequest request)
        {
            try
            {

                ApiService apiService = new ApiService(_configuration);

                var result = await apiService.FindArticle(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFindArticle - NewsController: " + ex);

                return null;
            }
        }

    }
}
