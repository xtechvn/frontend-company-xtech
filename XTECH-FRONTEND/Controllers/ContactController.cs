using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using XTECH_FRONTEND.Infrastructure.Utilities.Constants;
using XTECH_FRONTEND.Models.Contact;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;
using XTECH_FRONTEND.Services;

namespace XTECH_FRONTEND.Controllers
{
    public class ContactController : Controller
    {

        private readonly IConfiguration _configuration;
        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SendEmail(EmailContract model)
        {
            try
            {
                if (model.Email == null)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Email không được bỏ trống",

                    });
                }
                // Pattern regex để bắt định dạng email
                string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

                // Kiểm tra xem email có khớp với pattern không
                //bool isValidEmail = Regex.IsMatch(email, pattern);
                if (!Regex.IsMatch(model.Email, pattern))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Email nhập không đúng định dạng",

                    });
                }
              
                EmailService emailService = new EmailService(_configuration);
                var SendEmail = emailService.sendMail(model);
                if(SendEmail==true)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi thành công"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Gửi không thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    smg = "Đã xảy ra lỗi, vui lòng liên hệ IT"
                });

            }
            
        }
    }
}
