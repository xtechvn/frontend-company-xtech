using Microsoft.AspNetCore.Mvc;

namespace XTECH_FRONTEND.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
