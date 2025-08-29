using Microsoft.AspNetCore.Mvc;
using XTECH_FRONTEND.Controllers.API.CarRegistration.IRepositories;

namespace XTECH_FRONTEND.Controllers.Demo
{
    public class CarRegistrationController : Controller
    {
        private readonly IMongoService _mongoService;
        public CarRegistrationController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult IndexV2()
        {

            return View();
        }
        public IActionResult ListData()
        {
            var data = _mongoService.GetList();
            ViewBag.Data = data;
            return View();
        }
    }
}
