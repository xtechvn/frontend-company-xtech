namespace XTECH_FRONTEND.Models
{
    public class RegisterSubmitModel
    {
        public string ClientName {  get; set; } 
        public string Email { get; set; }
        public string Phone {  get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
