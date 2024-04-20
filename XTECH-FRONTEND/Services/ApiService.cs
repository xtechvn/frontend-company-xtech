using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using XTECH_FRONTEND.Infrastructure.Utilities.Constants;
using XTECH_FRONTEND.Infrastructure.Utilities.Helpers;
using XTECH_FRONTEND.Models;
using XTECH_FRONTEND.Models.News;
using XTECH_FRONTEND.Models.News.GetCategory;
using XTECH_FRONTEND.Models.News.GetDetail;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;
using XTECH_FRONTEND.Utilities;

namespace XTECH_FRONTEND.Services
{
    public class ApiService
    {
        private readonly IConfiguration _configuration;

        public ApiService(IConfiguration configuration)
        {
            _configuration = configuration;
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
        #endregion news 


    }
}
