using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using XTECH_FRONTEND.Infrastructure.Utilities.Constants;
using XTECH_FRONTEND.Infrastructure.Utilities.Helpers;
using XTECH_FRONTEND.Models;
using XTECH_FRONTEND.Models.Account;
using XTECH_FRONTEND.Models.News;
using XTECH_FRONTEND.Models.News.FindArticle;
using XTECH_FRONTEND.Models.News.GetCategory;
using XTECH_FRONTEND.Models.News.GetDetail;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;
using XTECH_FRONTEND.Models.Tickets;
using XTECH_FRONTEND.Models.VPS;
using XTECH_FRONTEND.Utilities;
using XTECH_FRONTEND.Views.Crawl;

namespace XTECH_FRONTEND.Services
{
    public class ApiService
    {
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;

        public ApiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _redisService = new RedisConn(configuration);
        }
        private string ApiDomain => _configuration.GetSection("API")["Domain"];
        private string PrivateKey => _configuration.GetSection("API")["KEY"];
   
        private async Task<T> PostTokenAsync<T>(string route, object paramObj)
        {
            HttpClient httpClient = new HttpClient();

            var data = JsonConvert.SerializeObject(paramObj);
            var token = AdavigoHelper.Encode(data, PrivateKey);

            var request = new[]
            {
            new KeyValuePair<string, string>("token", token)
            };

            var url = ApiDomain + route;

            var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(request));
            var stringResult = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("API Error: " + stringResult);

            return JsonConvert.DeserializeObject<T>(stringResult);
        }

        // 1) Danh sách ticket của user
        public Task<BaseObjectResponse2<TicketListResponseDto>> GetMyTickets(
    string userId, int status = -1, int page = 1, int size = 20)
        {
            return PostTokenAsync<BaseObjectResponse2<TicketListResponseDto>>(
                SystemConstants.AdavigoApiRoutes.GetMyTickets,
                new { user_id = userId, status, page, size }
            );
        }

        // 2) Tạo ticket
        public Task<BaseObjectResponse2<CreateTicketResultVm>> CreateTicket(
    string userid, int serviceId, int departmentId, string subject, string content)
        {
            return PostTokenAsync<BaseObjectResponse2<CreateTicketResultVm>>(
                SystemConstants.AdavigoApiRoutes.CreateTicket,
                new
                {
                    user_id = userid,
                    service_id = serviceId,
                    department_id = departmentId,
                    subject = subject,
                    content = content
                }
            );
        }

        // 3) Lấy detail ticket
        public Task<BaseObjectResponse2<TicketDetailDtoFe>> GetTicketDetail(Guid ticketId)
        {
            return PostTokenAsync<BaseObjectResponse2<TicketDetailDtoFe>>(
                SystemConstants.AdavigoApiRoutes.GetTicketDetail, // "/api/ticket/get-ticket-detail.json"
                new { ticket_id = ticketId.ToString() }
            );
        }

        public Task<BaseObjectResponse2<object>> AddMessageAttachments(
    long messageId, List<string> urls, List<string> names)
        {
            urls ??= new();
            names ??= new();

            return PostTokenAsync<BaseObjectResponse2<object>>(
                SystemConstants.AdavigoApiRoutes.AddMessageAttachments, // bạn tạo route này ở API
                new
                {
                    message_id = messageId,
                    attach_files = urls.Zip(names, (url, name) => new { url, name })
                }
            );
        }


        // 4) Reply ticket (user chat)
        public Task<BaseObjectResponse2<TicketMessageVm>> ReplyTicket(
      Guid ticketId,
      string senderType,
      string senderId,
      string content,
      string contentHtml = "",
      List<string> attachFileUrls = null,
      List<string> attachFileNames = null
  )
        {
            attachFileUrls ??= new List<string>();
            attachFileNames ??= new List<string>();

            return PostTokenAsync<BaseObjectResponse2<TicketMessageVm>>(
                SystemConstants.AdavigoApiRoutes.ReplyTicket,
                new
                {
                    ticket_id = ticketId.ToString(),
                    sender_type = senderType, // "Customer"/"Agent"
                    sender_id = senderId,
                    content = content,
                    content_html = contentHtml,
                    attach_files = attachFileUrls.Zip(attachFileNames, (url, name) => new { url, name })
                }
            );
        }


        #region news
        public async Task<GetCategoryObjResponse> GetNewsCategory(long id)
        {
            try
            {
                
                HttpClient _httpClient = new HttpClient();
                GetCategoryObjResponse result = null;
                //var token = "OmYiIDZWKRUTOm4QBxV/cnc8";
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var j_param = new Dictionary<string, long>()
                {
                    {"confirm",id },
                };
                var data = JsonConvert.SerializeObject(j_param);
                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.GetNewsCategory;
                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<GetCategoryObjResponse>(stringResult);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsCategory - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Models.News.GetListByCategoryId.GetListByCategoryIdResponse> GetNewsByCategoryId(GetListByCategoryIdRequest requestObj)
        {
            try
            {
                var data = JsonConvert.SerializeObject(requestObj);
                HttpClient _httpClient = new HttpClient();
                Models.News.GetListByCategoryId.GetListByCategoryIdResponse result = null;
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var token = AdavigoHelper.Encode(data, PrivateKey);


                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                //var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.GetNewsByCategoryId;
                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.GetNewsByCategoryId;

                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<Models.News.GetListByCategoryId.GetListByCategoryIdResponse>(stringResult);

                    // send error log

                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsByCategoryId - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseObjectResponse2<DataClientReturnViewModel>> Login(AccountModel model) 
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpClient _httpClient = new HttpClient();
                BaseObjectResponse2<DataClientReturnViewModel> datareturn = new();

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.Login;

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    datareturn = JsonConvert.DeserializeObject<BaseObjectResponse2<DataClientReturnViewModel>>(stringResult);
                }
                return datareturn;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Login - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseObjectResponse2<DataClientReturnViewModel>> Register(RegisterSubmitModel model) 
        {
            try
            {
                var data = JsonConvert.SerializeObject(model);
                HttpClient _httpClient = new HttpClient();
                BaseObjectResponse2<DataClientReturnViewModel> result = null;
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var token = AdavigoHelper.Encode(data, PrivateKey);


                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.Register;

                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<BaseObjectResponse2<DataClientReturnViewModel>>(stringResult);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Register - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseObjectResponse2<GetNewDetailResponse>> GetNewsDetail(long id)
        {
            try
            {

                HttpClient _httpClient = new HttpClient();
                BaseObjectResponse2<GetNewDetailResponse> result = null;
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var j_param = new Dictionary<string, long>()
                {
                    {"article_id",id },
                };
                var data = JsonConvert.SerializeObject(j_param);
                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.GetNewsDetail;

                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<BaseObjectResponse2<GetNewDetailResponse>>(stringResult);

                    // send error log

                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsDetail - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        // 5) Đổi status ticket (User reopen / CMS close)
        public Task<BaseObjectResponse2<object>> ChangeTicketStatus(Guid ticketId, int status)
        {
            return PostTokenAsync<BaseObjectResponse2<object>>(
                SystemConstants.AdavigoApiRoutes.ChangeTicketStatus,
                new
                {
                    ticket_id = ticketId.ToString(),
                    status = status
                    // status: 0=Open, 1=InProgress, 2=Resolved, 3=Closed
                }
            );
        }
        public async Task<BaseObjectResponse<ProductGroupViewModel>> getproductcategorybyparentid(long id)
        {
            try
            {

                HttpClient _httpClient = new HttpClient();
                BaseObjectResponse<ProductGroupViewModel> result = null;
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var j_param = new Dictionary<string, long>()
                {
                    {"category_id",1007 },
                };
                var data = JsonConvert.SerializeObject(j_param);
                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.getproductcategorybyparentid;

                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<BaseObjectResponse<ProductGroupViewModel>>(stringResult);

                    // send error log

                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getproductcategorybyparentid - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<BaseDataPaginationObjectResponse<List<ArticleResponse>>> FindArticle(FindArticleRequest requestObj)
        {
            try
            {
                BaseDataPaginationObjectResponse<List<ArticleResponse>> result = null;
                HttpClient _httpClient = new HttpClient();
                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var data = JsonConvert.SerializeObject(requestObj);

                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token),
                };

                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.FindArticle;
                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));

                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<BaseDataPaginationObjectResponse<List<ArticleResponse>>>(stringResult);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsDetail - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion news 
        #region galaxy
        public async Task<galaxycloudModel> GetPriceGalaxy(VPSmodel data)
        {
            try
            {

                HttpClient _httpClient = new HttpClient();
                galaxycloudModel result = null;
                string cache_name = CacheName.Get_Price_Galaxy + data.CPU + "_" + data.Memory + "_" + data.SSD + "_" + data.net + "_" + data.nip + "_" + data.nMonth + "_" + data.quantity;
                
                try
                {
                    var data_redis = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    if(data_redis!=null)
                    result = JsonConvert.DeserializeObject <galaxycloudModel> (data_redis);

                }
                catch (Exception ex)
                {
                    LogHelper.InsertLogTelegram("redisService - GetPriceGalaxy - ApiService: " + ex);

                }
                if (result == null)
                {
                    var st2 = new Stopwatch();
                    st2.Start();
                    var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("galaxy")["Domain"] + "?type=vps&cpu=" + data.CPU + "&mem=" + data.Memory + "&ssd=" + data.SSD + "&net=" + data.net + "&nip=" + data.nip + "&nMonth=" + data.nMonth + "&quantity=" + data.quantity + "";
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    st2.Stop();
                    if (st2.ElapsedMilliseconds > 1000)
                    {
                        LogHelper.InsertLogTelegram("GetPriceGalaxy - ApiService: time= " + st2.ElapsedMilliseconds+ ".URL=type=vps&cpu=" + data.CPU + "&mem=" + data.Memory + "&ssd=" + data.SSD + "&net=" + data.net + "&nip=" + data.nip + "&nMonth=" + data.nMonth + "&quantity=" + data.quantity + "") ;
                    }
                    var stringResult = "";
                    if (response.IsSuccessStatusCode)
                    {
                        stringResult = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<galaxycloudModel>(stringResult);
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    }
                }
                
               
                
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPriceGalaxy - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<BaseResponse> BookingGalaxy(VPSmodel requestObj)
        {
            try
            {

                HttpClient _httpClient = new HttpClient();
                BaseResponse result = null;
          

                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];
                var data = JsonConvert.SerializeObject(requestObj);

                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token),
                };
                var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain"] + SystemConstants.AdavigoApiRoutes.bookinggalaxy;
                HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(request));
                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<BaseResponse>(stringResult);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPriceGalaxy - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion galaxy 
        #region crawlapi
        public async Task<ProductViewModel> CrawlApi(string url )
        {
            try
            {

                HttpClient _httpClient = new HttpClient();
                ProductViewModel result = null;


                var PrivateKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["KEY"];

                var j_param = new Dictionary<string, string>()
                {
                    {"product_code","B003CEWPHC" },
                    {"url", url},
                    {"shop_id", "app_crawl_page"},
                };
                var data = JsonConvert.SerializeObject(j_param);
                var token = AdavigoHelper.Encode(data, PrivateKey);
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token),
                };
                var url_api = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API")["Domain_craw_api"] + SystemConstants.AdavigoApiRoutes.CrawlApi;
                HttpResponseMessage response = await _httpClient.PostAsync(url_api, new FormUrlEncodedContent(request));
                var stringResult = "";
                if (response.IsSuccessStatusCode)
                {
                    stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ProductViewModel>(stringResult);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CrawlApi - ApiService: " + ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion crawlapi 
    }
}
