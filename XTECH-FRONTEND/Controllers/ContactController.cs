﻿using Microsoft.AspNetCore.Mvc;
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

                EmailService emailService = new EmailService(_configuration);
                var SendEmail = emailService.sendMail(model);
                if(SendEmail==true)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        smg = "Gửi thành công"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        smg = "Gửi không thành công"
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