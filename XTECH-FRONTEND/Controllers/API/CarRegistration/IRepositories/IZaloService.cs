using HuloToys_Service.Controllers.CarRegistration.Model;
using XTECH_FRONTEND.Controllers.API.CarRegistration.Model;

namespace XTECH_FRONTEND.Controllers.API.CarRegistration.IRepositories
{
    public interface IZaloService
    {
        Task<(bool Success, string Status)> SendRegistrationNotificationAsync(RegistrationRecord record);
        Task<ZaloUserData?> GetUserDetailByPhoneAsync(string phoneNumber);
        Task<(bool Success, string Message)> SendMessageToUserAsync(string userId, string message);
    }
}
