using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using System.Net.Http;

namespace HuloToys_Service.Repositories
{
    public class GoogleFormsService : IGoogleFormsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleFormsService> _logger;
        private readonly IConfiguration _configuration;

        public GoogleFormsService(HttpClient httpClient, ILogger<GoogleFormsService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SubmitToGoogleFormAsync(RegistrationRecord record)
        {
            try
            {
                // Google Forms submission can be implemented here if needed
                // For now, just log the submission
                _logger.LogInformation($"Google Form submission simulated for: {record.PhoneNumber}");
                await Task.Delay(100); // Simulate API call
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting to Google Form");
                return false;
            }
        }
    }
}
