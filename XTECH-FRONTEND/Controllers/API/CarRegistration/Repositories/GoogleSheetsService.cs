using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using Microsoft.Extensions.Caching.Memory;


namespace HuloToys_Service.Repositories
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<GoogleSheetsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId;
        private readonly string _sheetName;
        public GoogleSheetsService(IMemoryCache cache, ILogger<GoogleSheetsService> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;

            // Get configuration
            _spreadsheetId = _configuration["GoogleSheets:SpreadsheetId"]
                ?? throw new ArgumentNullException("GoogleSheets:SpreadsheetId not configured");
            _sheetName = _configuration["GoogleSheets:SheetName"] ?? "CarRegistrations";

            // Initialize Google Sheets service
            _sheetsService = InitializeSheetsService();
        }
        private SheetsService InitializeSheetsService()
        {
            try
            {
                // Method 1: Using Service Account (Recommended for server applications)
                var serviceAccountFile = _configuration["GoogleSheets:ServiceAccountFile"];
                if (!string.IsNullOrEmpty(serviceAccountFile) && File.Exists(serviceAccountFile))
                {
                    var credential = GoogleCredential.FromFile(serviceAccountFile)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);

                    return new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Car Registration API"
                    });
                }

                // Method 2: Using JSON key directly from configuration
                var serviceAccountJson = _configuration["GoogleSheets:ServiceAccountJson"];
                if (!string.IsNullOrEmpty(serviceAccountJson))
                {
                    var credential = GoogleCredential.FromJson(serviceAccountJson)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);

                    return new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Car Registration API"
                    });
                }

                throw new InvalidOperationException("Google Sheets credentials not configured properly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Google Sheets service");
                throw;
            }
        }
        public async Task<int> GetDailyQueueCountAsync()
        {
            try
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var cacheKey = $"daily_count_{today}";

                // Check cache first
                if (_cache.TryGetValue(cacheKey, out int cachedCount))
                {
                    _logger.LogInformation($"Retrieved daily queue count from cache: {cachedCount}");
                    return cachedCount;
                }

                // Get today's date range for filtering
                var todayStart = DateTime.Today;
                var todayEnd = DateTime.Today.AddDays(1);

                // Read all data from the sheet
                var range = $"{_sheetName}!A2:D2"; // Assuming columns: A=Phone, B=Plate, C=Queue, D=DateTime
                var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = await request.ExecuteAsync();
                var values = response.Values;

                if (values == null || values.Count <= 1) // No data or only headers
                {
                    _logger.LogInformation("No registration data found for today");
                    _cache.Set(cacheKey, 0, TimeSpan.FromHours(1)); // Cache for 1 hour
                    return 0;
                }

                var count = 0;

                // Skip header row (index 0) and count rows with today's date
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];

                    // Check if row has enough columns and datetime column is not empty
                    if (row.Count >= 4 && !string.IsNullOrEmpty(row[3]?.ToString()))
                    {
                        // Try to parse the datetime from column D (index 3)
                        if (DateTime.TryParse(row[3].ToString(), out DateTime registrationDate))
                        {
                            // Check if registration date is today
                            if (registrationDate.Date == todayStart.Date)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Invalid date format in row {i + 1}: {row[3]}");
                        }
                    }
                }

                _logger.LogInformation($"Retrieved daily queue count from Google Sheets: {count}");

                // Cache the result for 5 minutes (to balance performance and accuracy)
                _cache.Set(cacheKey, count, TimeSpan.FromMinutes(5));

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily queue count from Google Sheets");

                // Return cached value if available, otherwise throw
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var cacheKey = $"daily_count_{today}";

                if (_cache.TryGetValue(cacheKey, out int cachedCount))
                {
                    _logger.LogWarning("Returning cached value due to error");
                    return cachedCount;
                }

                throw;
            }
        }

        public async Task<bool> SaveRegistrationAsync(RegistrationRecord record)
        {
            try
            {
                // Prepare the data to append
                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        record.PhoneNumber,
                        record.PlateNumber,
                        record.QueueNumber,
                        record.RegistrationTime.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                };

                var valueRange = new ValueRange
                {
                    Values = values
                };

                // Append to the sheet
                var appendRequest = _sheetsService.Spreadsheets.Values.Append(
                    valueRange,
                    _spreadsheetId,
                    $"{_sheetName}!A:D");

                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

                var appendResponse = await appendRequest.ExecuteAsync();

                if (appendResponse.Updates.UpdatedRows.HasValue && appendResponse.Updates.UpdatedRows.Value > 0)
                {
                    _logger.LogInformation($"Successfully saved registration to Google Sheets: {record.PhoneNumber} - {record.PlateNumber} - Queue: {record.QueueNumber}");

                    // Update the daily count cache
                    var today = DateTime.Today.ToString("yyyy-MM-dd");
                    var cacheKey = $"daily_count_{today}";
                    if (_cache.TryGetValue(cacheKey, out int currentCount))
                    {
                        _cache.Set(cacheKey, currentCount + 1, TimeSpan.FromMinutes(5));
                    }

                    return true;
                }

                _logger.LogWarning("No rows were updated when saving to Google Sheets");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving registration to Google Sheets");
                return false;
            }
        }

        public async Task<DateTime?> GetLastSubmissionTimeAsync(string phoneNumber)
        {
            try
            {
                var cacheKey = $"last_submission_{phoneNumber}";

                // Check memory cache first
                if (_cache.TryGetValue(cacheKey, out DateTime cachedTime))
                {
                    return cachedTime;
                }

                // Query Google Sheets for the most recent submission from this phone number
                var range = $"{_sheetName}!A:D";
                var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = await request.ExecuteAsync();
                var values = response.Values;

                if (values == null || values.Count <= 1)
                {
                    return null;
                }

                DateTime? lastSubmission = null;

                // Search through all rows for this phone number
                for (int i = values.Count - 1; i >= 1; i--) // Start from the end (most recent)
                {
                    var row = values[i];

                    if (row.Count >= 4 &&
                        row[0]?.ToString() == phoneNumber &&
                        !string.IsNullOrEmpty(row[3]?.ToString()))
                    {
                        if (DateTime.TryParse(row[3].ToString(), out DateTime submissionTime))
                        {
                            lastSubmission = submissionTime;
                            break; // Found the most recent submission
                        }
                    }
                }

                if (lastSubmission.HasValue)
                {
                    // Cache for 15 minutes
                    _cache.Set(cacheKey, lastSubmission.Value, TimeSpan.FromMinutes(15));
                    _logger.LogInformation($"Found last submission for {phoneNumber}: {lastSubmission}");
                }

                return lastSubmission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting last submission time for {phoneNumber}");
                return null;
            }
        }

        public async Task UpdateLastSubmissionTimeAsync(string phoneNumber, DateTime submissionTime)
        {
            try
            {
                var cacheKey = $"last_submission_{phoneNumber}";

                // Update memory cache
                _cache.Set(cacheKey, submissionTime, TimeSpan.FromMinutes(15));

                _logger.LogInformation($"Updated last submission time for {phoneNumber}: {submissionTime}");

                // Note: The actual time is already saved in SaveRegistrationAsync,
                // so we don't need to make another API call here
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating last submission time for {phoneNumber}");
                throw;
            }
        }

        // Helper method to ensure sheet has headers
        public async Task<bool> EnsureSheetHeadersAsync()
        {
            try
            {
                var range = $"{_sheetName}!A1:D1";
                var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var response = await request.ExecuteAsync();

                // If no headers exist, create them
                if (response.Values == null || response.Values.Count == 0)
                {
                    var headers = new List<IList<object>>
                    {
                        new List<object> { "Số điện thoại", "Biển số xe", "Số thứ tự", "Ngày giờ đăng ký" }
                    };

                    var valueRange = new ValueRange { Values = headers };

                    var updateRequest = _sheetsService.Spreadsheets.Values.Update(
                        valueRange,
                        _spreadsheetId,
                        range);

                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                    await updateRequest.ExecuteAsync();
                    _logger.LogInformation("Created headers in Google Sheet");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring sheet headers");
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sheetsService?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
