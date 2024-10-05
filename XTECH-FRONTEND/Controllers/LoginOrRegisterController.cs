using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Ultilities.Constants;
using XTECH_FRONTEND.Models;
using XTECH_FRONTEND.Models.Account;
using XTECH_FRONTEND.Services;

namespace XTECH_FRONTEND.Controllers
{
    public class LoginOrRegisterController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginOrRegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ConfirmLogin(AccountModel model)
        {
            try 
            {
                ApiService apiService = new ApiService(_configuration);
                BaseObjectResponse2<DataClientReturnViewModel> RS = await apiService.Login(model);
                if (model.RememberMe) 
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, RS.data.IdClient.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, RS.data.UserName));
                    claims.Add(new Claim(ClaimTypes.Email, RS.data.Email));
                    claims.Add(new Claim("AccountId", RS.data.IdAccount.ToString()));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                        IsPersistent = true
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                }
                return Ok(RS.data);
            }
            catch (Exception ex) 
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    smg = "Đăng nhập thất bại!"
                });
            }
        }

        public async Task<IActionResult> SignOut(AccountModel model)
        {
            try
            {
                await HttpContext.SignOutAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    smg = "Đăng nhập thất bại!"
                });
            }
        }

        public async Task<string> GetUserNameClaims()
        {
            try
            {
                var _UserNameLogin = "";
                if (HttpContext.User.FindFirst(ClaimTypes.Name) != null)
                {
                    _UserNameLogin = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                }
                return _UserNameLogin;
            }
            catch 
            {
                return null;
            }
        }
    }


}
