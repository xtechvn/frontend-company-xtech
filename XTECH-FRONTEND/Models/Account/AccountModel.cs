using System.ComponentModel.DataAnnotations;

namespace XTECH_FRONTEND.Models.Account
{
    public class AccountModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}
