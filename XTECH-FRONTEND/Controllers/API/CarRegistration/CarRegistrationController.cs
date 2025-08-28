
using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XTECH_FRONTEND.Controllers.API.CarRegistration.IRepositories;
using XTECH_FRONTEND.Services;
using XTECH_FRONTEND.Utilities;


namespace XTECH_FRONTEND.Controllers.CarRegistration
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarRegistrationController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleFormsService _googleFormsService;
        private readonly IZaloService _zaloService;
        private readonly ILogger<CarRegistrationController> _logger;
        private readonly IMongoService _mongoService;
        private readonly RedisConn redisService;
        private readonly IConfiguration _configuration;
        private readonly WorkQueueClient _workQueueClient;

        public CarRegistrationController(
            IValidationService validationService,
            IGoogleSheetsService googleSheetsService,
            IGoogleFormsService googleFormsService,
            IZaloService zaloService,
            ILogger<CarRegistrationController> logger,
            IMongoService mongoService, IConfiguration configuration)
        {
            _validationService = validationService;
            _googleSheetsService = googleSheetsService;
            _googleFormsService = googleFormsService;
            _zaloService = zaloService;
            _logger = logger;
            _mongoService = mongoService;
            redisService = new RedisConn(configuration);
            redisService.Connect();
            _configuration = configuration;
            _workQueueClient = new WorkQueueClient(configuration);
        }
        [HttpPost("register")]
        public async Task<ActionResult<CarRegistrationResponse>> RegisterCar([FromBody] CarRegistrationRequest request)
        {
            try
            {
                var now =  DateTime.Now;
                var hours = now.Hour;
                var minutes = now.Minute;

                // Kiểm tra khoảng 17:55 đến 18:00
                if (hours == 17 && minutes >= 55)
                {
                    return StatusCode(500, new CarRegistrationResponse
                    {
                        Success = false,
                        Message = "Vui lòng đợi đến 18 giờ đăng lý lại "
                    });
                }
                    _logger.LogInformation($"Car registration request received: {request.PhoneNumber} - {request.PlateNumber}");

                // Step 1: Validate input data
                var validationResult = _validationService.ValidateCarRegistration(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = string.Join(", ", validationResult.Errors)
                    });
                }

                // Step 2: Check time restriction (15 minutes rule)
                var timeRestriction = _validationService.CheckTimeRestriction(request.PlateNumber);
                if (!timeRestriction.CanSubmit)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = $"Vui lòng đợi {timeRestriction.RemainingMinutes} phút trước khi gửi lại",
                        RemainingTimeMinutes = timeRestriction.RemainingMinutes
                    });
                }

                // Step 3: Get current daily queue count
                var dailyCount = await _googleSheetsService.GetDailyQueueCountAsync();
                var queueNumber = dailyCount + 1;

                // Step 4: Create registration record with initial Zalo status
                var registrationRecord = new RegistrationRecord
                {
                    PhoneNumber = request.PhoneNumber,
                    PlateNumber = request.PlateNumber.ToUpper(),
                    Name = request.Name.ToUpper(),
                    Referee = request.Referee.ToUpper(),
                    GPLX = request.GPLX.ToUpper(),
                    QueueNumber = queueNumber,
                    RegistrationTime = DateTime.Now,
                    ZaloStatus = "Đang xử lý...",
                    Camp= request.Camp
                };

                // Step 5: Submit to Google Form
                var formSubmissionSuccess = await _googleFormsService.SubmitToGoogleFormAsync(registrationRecord);
                if (!formSubmissionSuccess)
                {
                    _logger.LogWarning("Google Form submission failed, but continuing...");
                }

                // Step 6: Send Zalo notification and get status
                var (zaloSuccess, zaloStatus) = await _zaloService.SendRegistrationNotificationAsync(registrationRecord);

                // Update registration record with Zalo status
                registrationRecord.ZaloStatus = zaloStatus;

                // Step 7: Save to mogoDB
                await _mongoService.Insert(registrationRecord);
                // Step 7: Save to Google Sheets with Zalo status
                var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(registrationRecord);
                if (!sheetsSuccess)
                {
                    return StatusCode(500, new CarRegistrationResponse
                    {
                        Success = false,
                        Message = "Lỗi hệ thống, vui lòng thử lại sau"
                    });
                }

                // Step 8: Update last submission time
                await _googleSheetsService.UpdateLastSubmissionTimeAsync(request.PlateNumber, DateTime.Now);

                // Return success response
                return Ok(new CarRegistrationResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    QueueNumber = queueNumber,
                    RegistrationTime = registrationRecord.RegistrationTime,
                    PlateNumber = registrationRecord.PlateNumber,
                    PhoneNumber = registrationRecord.PhoneNumber,
                    ZaloStatus = zaloStatus,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing car registration");
                return StatusCode(500, new CarRegistrationResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau"
                });
            }
        }

        [HttpGet("check-restriction/{plateNumber}")]
        public ActionResult<TimeRestrictionResult> CheckTimeRestriction(string PlateNumber)
        {
            try
            {
                var result = _validationService.CheckTimeRestriction(PlateNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking time restriction for {PlateNumber}");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet("queue-status")]
        public async Task<ActionResult<object>> GetQueueStatus()
        {
            try
            {
                var dailyCount = await _googleSheetsService.GetDailyQueueCountAsync();
                return Ok(new
                {
                    CurrentQueueNumber = dailyCount,
                    NextQueueNumber = dailyCount + 1,
                    Date = DateTime.Today.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting queue status");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
        [HttpGet("check-zalo-user/{phoneNumber}")]
        public async Task<ActionResult> CheckZaloUser(string phoneNumber)
        {
            try
            {
                var userDetail = await _zaloService.GetUserDetailByPhoneAsync(phoneNumber);

                if (userDetail == null)
                {
                    return Ok(new
                    {
                        exists = false,
                        message = "Số điện thoại này chưa được Approve Zalo OA"
                    });
                }

                return Ok(new
                {
                    exists = true,
                    userId = userDetail.user_id,
                    displayName = userDetail.display_name,
                    isFollower = userDetail.user_is_follower,
                    lastInteraction = userDetail.user_last_interaction_date,
                    avatar = userDetail.Avatar,
                    status = userDetail.user_is_follower ? "Có thể gửi tin nhắn" : "Chưa follow OA"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking Zalo user for {phoneNumber}");
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
        [HttpPost("registerV2")]
        public async Task<ActionResult<CarRegistrationResponse>> RegisterCarV2([FromBody] CarRegistrationRequest request)
        {
            try
            {
                var now = DateTime.Now;
                var hours = now.Hour;
                var minutes = now.Minute;

                // Kiểm tra khoảng 17:55 đến 18:00
                if (hours == 17 && minutes >= 55)
                {
                    return StatusCode(500, new CarRegistrationResponse
                    {
                        Success = false,
                        Message = "Vui lòng đợi đến 18 giờ đăng lý lại "
                    });
                }
                _logger.LogInformation($"Car registration request received: {request.PhoneNumber} - {request.PlateNumber}");

                // Step 1: Validate input data
                var validationResult = _validationService.ValidateCarRegistration(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = string.Join(", ", validationResult.Errors)
                    });
                }

                string cache_name = "PlateNumber_" + request.PlateNumber.Replace("-", "_");
                var data = redisService.Get(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                if (data != null && data.Trim() != "")
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = $"Biển số xe đã đăng ký, Vui lòng đợi 15 phút trước khi gửi lại",
                        RemainingTimeMinutes = 15
                    });
                }
                redisService.Set(cache_name, JsonConvert.SerializeObject(request), DateTime.Now.AddMinutes(15), Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                // Step 3: Get current daily queue count
          
                var queueNumber = 0;

                // Step 4: Create registration record with initial Zalo status
                var registrationRecord = new RegistrationRecord
                {
                    PhoneNumber = request.PhoneNumber,
                    PlateNumber = request.PlateNumber.ToUpper(),
                    Name = request.Name.ToUpper(),
                    Referee = request.Referee.ToUpper(),
                    GPLX = request.GPLX.ToUpper(),
                    QueueNumber = queueNumber,
                    RegistrationTime = DateTime.Now,
                    ZaloStatus = "Đang xử lý...",
                    Camp = request.Camp
                };

 

                // Step 6: Send Zalo notification and get status
                var (zaloSuccess, zaloStatus) = await _zaloService.SendRegistrationNotificationAsync(registrationRecord);

                // Update registration record with Zalo status
                registrationRecord.ZaloStatus = zaloStatus;



                _workQueueClient.SyncQueue(registrationRecord);

                while (queueNumber <= 0)
                {
                    var data_Redis = redisService.Get(
                        cache_name,
                        Convert.ToInt32(_configuration["Redis:Database:db_common"])
                    );

                    if (!string.IsNullOrEmpty(data_Redis))
                    {
                        var data_detail = JsonConvert.DeserializeObject<RegistrationRecord>(data_Redis);

                        if (data_detail?.QueueNumber > 0)
                        {
                            queueNumber = data_detail.QueueNumber;
                            break; // đã có, thoát loop
                        }
                    }
                    Thread.Sleep(2000); // nghỉ 200ms rồi thử lại
                }

                // Return success response
                return Ok(new CarRegistrationResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    QueueNumber = queueNumber,
                    RegistrationTime = registrationRecord.RegistrationTime,
                    PlateNumber = registrationRecord.PlateNumber,
                    PhoneNumber = registrationRecord.PhoneNumber,
                    ZaloStatus = zaloStatus,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing car registration");
                return StatusCode(500, new CarRegistrationResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau"
                });
            }
        }

    }
}
