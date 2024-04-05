using Newtonsoft.Json;
using XTECH_FRONTEND.Infrastructure.Utilities.Constants;
using XTECH_FRONTEND.Infrastructure.Utilities.Helpers;
using XTECH_FRONTEND.Models;
using XTECH_FRONTEND.Models.News.GetDetail;
using XTECH_FRONTEND.Models.News.GetListByCategoryId;

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
        public async Task<Models.News.GetListByCategoryId.GetListByCategoryIdResponse> GetNewsCategory(GetListByCategoryIdRequest requestObj)
        {
            try
            {
                var data = JsonConvert.SerializeObject(requestObj);
                HttpClient _httpClient = new HttpClient();
                Models.News.GetListByCategoryId.GetListByCategoryIdResponse result = null;
                var PrivateKey= SystemConstants.AdavigoApiRoutes.api_manual;
                var token = AdavigoHelper.Encode(data,PrivateKey);
             
          
                var request = new[]
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var url = SystemConstants.AdavigoApiRoutes.Domain_API + SystemConstants.AdavigoApiRoutes.GetNewsByCategoryId;
               
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
               
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseObjectResponse2<GetNewDetailResponse>> GetNewsDetail(long id)
        {
            try
            {
              
                HttpClient _httpClient = new HttpClient();
                BaseObjectResponse2<GetNewDetailResponse> result = null;
                var PrivateKey = SystemConstants.AdavigoApiRoutes.api_manual;
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

                var url = SystemConstants.AdavigoApiRoutes.Domain_API + SystemConstants.AdavigoApiRoutes.GetNewsDetail;

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

                throw new Exception(ex.Message);
            }
        }
        #endregion news 


    }
}
