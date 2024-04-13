using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Xml.Linq;

namespace XTECH_FRONTEND.Controllers
{
    public class HomeController : Controller
    {       
        // GET: /home/
        public IActionResult Index()
        {
           return View();           
        }
        //home/welcome?a=hello&b=2
        public string Welcome(string a,int  b=1)
        {
            return HtmlEncoder.Default.Encode($"Hello {a}, NumTimes is: {b}");
        }       

    }
}
