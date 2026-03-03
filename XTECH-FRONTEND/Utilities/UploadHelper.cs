

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using XTECH_FRONTEND.Utilities;
using static System.Net.Mime.MediaTypeNames;


namespace B2B.Utilities.Common
{
    public class UpLoadHelper
    {
        
        static string apiUploadFile = "https://static-image.adavigo.com/File/Upload";
       
        static string key_token_api = "wVALy5t0tXEgId5yMDNg06OwqpElC9I0sxTtri4JAlXluGipo6kKhv2LoeGQnfnyQlC07veTxb7zVqDVKwLXzS7Ngjh1V3SxWz69";
        /// <summary>
        /// UploadImageBase64
        /// </summary>
        /// <param name="ImageBase64">src of image</param>
        /// <returns></returns>
        /// 
        static string AES_KEY = "bXny0OMniop5U1fpZ6u7zAHg7KxRdUcp7OuhU7L9Oo62k+FLShyDuwjBGuXWtRMK";
        static string AES_IV = "KFavGEDPdhddqjl9CQVC2c0jYoMJKzmqlBDS+JBbSK6QwgG79XWs9ltH0i5DaJm2";

        public static async Task<string> UploadFileOrImage(IFormFile file, long dataId, int type)
        {
            if (file == null || file.Length <= 0)
                throw new Exception("File không hợp lệ.");

            try
            {
                // Danh sách phần mở rộng hợp lệ
                var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var validFileExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx" };

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!validImageExtensions.Contains(extension) && !validFileExtensions.Contains(extension))
                {
                    throw new Exception($"Định dạng file {extension} không được hỗ trợ.");
                }

                byte[] AESKey = EncryptService.Get_AESKey(EncryptService.ConvertBase64StringToByte(AES_KEY));
                byte[] AESIV = EncryptService.Get_AESIV(EncryptService.ConvertBase64StringToByte(AES_IV));

                string token = GenerateToken(AESKey, AESIV);

                using var formData = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();

                formData.Add(new StreamContent(fileStream), "data", file.FileName);
                formData.Add(new StringContent(file.FileName), "name");
                formData.Add(new StringContent(dataId.ToString()), "data_id");
                formData.Add(new StringContent(type.ToString()), "type");
                formData.Add(new StringContent(token), "token");

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(apiUploadFile, formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                if (response.IsSuccessStatusCode && jsonResponse?.status == 0)
                {
                    return $"https://static-image.adavigo.com{jsonResponse.url}";
                }

                LogHelper.InsertLogTelegram($"UploadFileOrImage Failed: {jsonResponse?.msg}");
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram($"UploadFileOrImage Exception: {ex.Message}");
                throw;
            }
        }


        private static string GenerateToken(byte[] AESKey, byte[] AESIV)
        {
            try
            {
                var currentTime = DateTime.UtcNow.ToString("o");
                return EncryptService.ConvertByteToBase64String(EncryptService.AES_EncryptToByte(currentTime, AESKey, AESIV));
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram($"GenerateToken Error: {ex.Message}");
                return null;
            }
        }


        

    }
}
