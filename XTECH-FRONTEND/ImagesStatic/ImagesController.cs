using Entities.ViewModels.Static;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Ultilities.Constants;

namespace Utilities
{

    [Route("static/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private IConfiguration configuration;
        private readonly string[] stringArray = { "PNG", "JPG", "JPEG", "GIF", "BMP" };
        public ImagesController(IConfiguration _configuration)
        {
            configuration = _configuration;
        
        }


        // GET: MediaController

        [HttpPost("upload")]
        public IActionResult Upload([FromBody] object body)
        {
            try
            {
                var Jobject = JObject.Parse(body.ToString());
                string token = Jobject["token"].ToString();

                string[] stringArray = { "PNG", "JPG", "JPEG", "GIF", "BMP" };
                //Max Size file có thể upload: mặc định là 3MB
                int max_file_size = 3 * 1024 * 1024;

                try
                {
                    if (Convert.ToInt32(configuration["ConfigSize:General"]) > 0)
                    {
                        max_file_size = Convert.ToInt32(configuration["ConfigSize:General"]);
                    }
                }
                catch (FormatException)
                {

                }

                //Decode token để lấy JSON:
                string param = CommonHelper.Decode(token, configuration["Config:Static"]);

                //Model hóa JSON:
                ImageDetail img_detail = JsonConvert.DeserializeObject<ImageDetail>(param);

                // Nếu extend thuộc ảnh
                if (!stringArray.Contains(img_detail.extend.ToUpper()))
                {
                    //Trả kết quả sai file type:
                    return BadRequest(new { status = (int)ResponseType.ERROR, message = "Invalid File Type", url_path = "" });
                }

                //Nếu lấy ra được thông tin:
                if (ImageUploadHelper.IsBase64String(img_detail.data_file))
                {
                    string root =  configuration["File:MainFolder"] ;
                    List<string> child = new List<string>();
                    child.Add(configuration["File:Images"]);
                   
                    //Lấy thông tin thời gian hiện tại
                    DateTime time = DateTime.Now;
                    string year = time.Year.ToString();
                    string month = time.Month.ToString();
                    string day = time.Day.ToString();
                    //Thông tin file và build đường dẫn local:
                    child.Add(year);
                    child.Add(month);
                    child.Add(day);

                    string file_name = Guid.NewGuid() + "." + img_detail.extend;
                    string folder = FileService.CheckAndCreateFolder(root, child);



                    string imgPath_full = folder+"\\" + file_name;
                    byte[] bytes = System.Convert.FromBase64String(img_detail.data_file);
                    //Kiểm tra nếu file vượt quá max size:
                    if (bytes.Length > max_file_size)
                    {
                        return BadRequest(new { status = (int)ResponseType.ERROR, message = "The file image exceeds the maximum allowed size: " + max_file_size + " bytes", file_path = file_name, url_path = "" });
                    }
                    else
                    {
                        //Ghi byte[] vào file đã tạo:
                        using (var fs = new FileStream(imgPath_full, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    //Build đường link local:
                    string urlPath_full = FileService.BuildURLFromPath(imgPath_full);

                    //Trả kết quả
                    return Ok(new { status = (int)ResponseType.SUCCESS, message = "Images Received", url_path = urlPath_full,url_path_full= imgPath_full });
                }
                //Thông tin không được encode với key trong file .config hoặc thông tin convert ra null:
                else
                {
                    return BadRequest(new { status = (int)ResponseType.ERROR, message = "Invalid Format", url_path = "" });
                }
            }
            catch (Exception e)
            {
                //Log Telegram:
                //LogHelper.InsertLogTelegram("ServiceReceiverMedia - Upload Error: " + e.ToString());

                //Lỗi trên API
                return BadRequest(new { status = (int)ResponseType.ERROR, message = "On Execution", url_path = "" });
            }
        }
        [HttpPost("upload-payment")]
        public IActionResult UploadPaymentImage([FromBody] object body)
        {
            try
            {
                var Jobject = JObject.Parse(body.ToString());
                string token = Jobject["token"].ToString();
                /*
                var object_input= new PaymentImageDetail()
                {
                    data_file = "iVBORw0KGgoAAAANSUhEUgAAALwAAABSCAIAAABhZSkOAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAPOSURBVHhe7ZpNjuIwEEbnICw5DSvOwoKzIPVREEdpbtALxAIhITF2lePYIZB8zEzTTN6TFyRx/PtSdiJ+XQFEkAZkkAZkkAZkkAZkkAZkkAZkkAZkkAZkkAZknpTmvJp/zZbHzT4dw5T4g0jz+XGYLU+f6Qimw7PSbNch0hxWu0s6hgnxrDQhzCw+MGaafPdG+LJZfs3mx206hHfkOWn2p0XYCM+/9GCDNCVhNA6z9TkdvQ1PSRO3wPPjKm5r1I0w0pTYS+g0pLGJD13dHWfzg/jWjTQl05HG1qbVLv2oVyhzIoSf/XmzDtHIlrB1GY2yNJc7GQZpqnhYwufutAqR364Gsxfrc85gYfJ2YfUvT7Ecj6PbcCY21XLuQ2mxnNjrTNHHmxfJgXGI755+YzepD+FL0KWxMfW+5fnLuBMH3/G0qX2Y/JbDImbrzTBIU0J5e0hlM5otV5VyFX61avbVoqY9CU0H2xYuj26P/c53mWR1KkQcGIepSVOJUgjk+GDFzqcnb++Dm9ejwQyD9JTgM9Q2IwSG9WnbHnarsDmrpsfOpAweijxDM7vhkteb8vj5NrqEKoqro7tpJ///5amzJHVXqDRYZRivZ2gwwyBeQp3f4sTNitNSOhHp5o+Tlw9dGj90OYrfXkjfZBexanQ3pyHNTWixbrdB2werep7qwRrMMEhPCTfuhj3NsbsCVrdYIXfiZXlYtq2Vpnf5s9S0YWQ3JyGNj8VtymMxOFgjR/MBPSV0pGnWl06qbinMsAKLmUOaISRp/sJg/RNp3JJmLbCZCAXmDUc535nsmf0o15FhaayK7GgfijSdLfkboEhTz02m7Pk3StPkjytRtCSX6dLkDJftRydDwiXYhE7V0zZCGm9D3OTeme+R3UzllH6/A4o0xaiVND2PwzE4WCNH8wFeQjcVKvdn6Gm5bV1D6sSMEdLcC7q5irHdtJNlGj8OL0SQxp7gvgW4iEAvkGaxPN48qe13P//sto0trCo1PCZ1qx4lTSC+Zje1pJSvju/mZbsuP+eMH4cXom2EfwI98/Esdx8DeMhkpUmf497iyf5pTE8aWyZSevgGBPeYrDT8V/V53k8aeDlIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAzJIAyLX629jb4GkrwldmwAAAABJRU5ErkJggg==",
                    extend="jpg"
                };
                token = CommonHelper.Encode(JsonConvert.SerializeObject(object_input), configuration["key_api:b2b"]);
                */

                //Array kiểm tra nếu file name là file ảnh thông thường.
                string[] stringArray = { "PNG", "JPG", "JPEG", "GIF", "BMP" };
                //Max Size file có thể upload: mặc định là 6MB
                int max_file_size = 6 * 1024 * 1024;

                try
                {
                    if (Convert.ToInt32(configuration["ConfigSize:General"]) > 0)
                    {
                        max_file_size = Convert.ToInt32(configuration["ConfigSize:General"]);
                    }
                }
                catch (FormatException)
                {

                }

                //Decode token để lấy JSON:
                string param = CommonHelper.Decode(token, configuration["DataBaseConfig:key_api:b2b"]);

                //Model hóa JSON:
                PaymentImageDetail img_detail = JsonConvert.DeserializeObject<PaymentImageDetail>(param);

                // Nếu extend thuộc video
                if (!stringArray.Contains(img_detail.extend.ToUpper()))
                {
                    //Trả kết quả sai file type:
                    return BadRequest(new { status = (int)ResponseType.ERROR, message = "Invalid File Type", url_path = "" });
                }

                //Nếu lấy ra được thông tin:
                if (ImageUploadHelper.IsBase64String(img_detail.data_file))
                {
                    string root = configuration["File:MainFolder"];
                    List<string> child = new List<string>();
                    child.Add(configuration["File:Images"]);

                    //Lấy thông tin thời gian hiện tại
                    DateTime time = DateTime.Now;
                    string year = time.Year.ToString();
                    string month = time.Month.ToString();
                    string day = time.Day.ToString();
                    //Thông tin file và build đường dẫn local:
                    child.Add(year);
                    child.Add(month);
                    child.Add(day);

                    string file_name = Guid.NewGuid() + "." + img_detail.extend;
                    string folder = FileService.CheckAndCreateFolder(root, child);



                    string imgPath_full = folder + "\\" + file_name;

                    byte[] bytes = System.Convert.FromBase64String(img_detail.data_file);
                    //Kiểm tra nếu file vượt quá max size:
                    if (bytes.Length > max_file_size)
                    {
                        return BadRequest(new { status = (int)ResponseType.ERROR, message = "The file image exceeds the maximum allowed size: " + max_file_size + " bytes", file_path = file_name, url_path = "" });
                    }
                    else
                    {
                        //Ghi byte[] vào file đã tạo:
                        using (var fs = new FileStream(imgPath_full, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    //Build đường link local:
                    string urlPath_full = FileService.BuildURLFromPath(imgPath_full);

                    //Trả kết quả
                    return Ok(new { status = (int)ResponseType.SUCCESS, message = "Images Received", url_path = urlPath_full});
                }
                //Thông tin không được encode với key trong file .config hoặc thông tin convert ra null:
                else
                {
                    return BadRequest(new { status = (int)ResponseType.FAILED, message = "Invalid Format", url_path = "" });
                }
            }
            catch (Exception e)
            {
                //Log Telegram:
               // LogHelper.InsertLogTelegram("ServiceReceiverMedia - Upload Error: " + e.ToString());

                //Lỗi trên API
                return BadRequest(new { status = (int)ResponseType.ERROR, message = "On Execution", url_path = "" });
            }
        }

        [HttpPost("upload-with-name")]
        public IActionResult UploadAndKeepFileName([FromBody] object body)
        {
            try
            {
                var Jobject = JObject.Parse(body.ToString());
                string token = Jobject["token"].ToString();

                string[] stringArray = { "PNG", "JPG", "JPEG", "GIF", "BMP" };
                //Max Size file có thể upload: mặc định là 3MB
                int max_file_size = 3 * 1024 * 1024;

                try
                {
                    if (Convert.ToInt32(configuration["ConfigSize:General"]) > 0)
                    {
                        max_file_size = Convert.ToInt32(configuration["ConfigSize:General"]);
                    }
                }
                catch (FormatException)
                {

                }

                //Decode token để lấy JSON:
                string param = CommonHelper.Decode(token, configuration["Config:Static"]);

                //Model hóa JSON:
                TicketImageDetail img_detail = JsonConvert.DeserializeObject<TicketImageDetail>(param);

                // Nếu extend thuộc ảnh
                if (!stringArray.Contains(img_detail.extend.ToUpper()))
                {
                    //Trả kết quả sai file type:
                    return BadRequest(new { status = (int)ResponseType.ERROR, message = "Invalid File Type", url_path = "" });
                }

                //Nếu lấy ra được thông tin:
                if (ImageUploadHelper.IsBase64String(img_detail.data_file))
                {
                    string root = configuration["File:MainFolder"];
                    List<string> child = new List<string>();
                    child.Add(configuration["File:Images"]);

                    //Lấy thông tin thời gian hiện tại
                    DateTime time = DateTime.Now;
                    string year = time.Year.ToString();
                    string month = time.Month.ToString();
                    string day = time.Day.ToString();
                    //Thông tin file và build đường dẫn local:
                    child.Add(year);
                    child.Add(month);
                    child.Add(day);

                    string file_name = Guid.NewGuid() + "." + img_detail.extend;
                    string folder = FileService.CheckAndCreateFolder(root, child);



                    string imgPath_full = folder + "\\" + file_name;

                    byte[] bytes = System.Convert.FromBase64String(img_detail.data_file);
                    //Kiểm tra nếu file vượt quá max size:
                    if (bytes.Length > max_file_size)
                    {
                        return BadRequest(new { status = (int)ResponseType.ERROR, message = "The file image exceeds the maximum allowed size: " + max_file_size + " bytes", file_path = file_name, url_path = "" });
                    }
                    else
                    {
                        //Ghi byte[] vào file đã tạo:
                        using (var fs = new FileStream(imgPath_full, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    //Build đường link local:
                    string urlPath_full = FileService.BuildURLFromPath(imgPath_full);

                    //Trả kết quả
                    return Ok(new { status = (int)ResponseType.SUCCESS, message = "Images Received", url_path = urlPath_full});
                }
                //Thông tin không được encode với key trong file .config hoặc thông tin convert ra null:
                else
                {
                    return BadRequest(new { status = (int)ResponseType.ERROR, message = "Invalid Format", url_path = "" });
                }
            }
            catch (Exception e)
            {
                //Log Telegram:
                //LogHelper.InsertLogTelegram("ServiceReceiverMedia - Upload Error: " + e.ToString());

                //Lỗi trên API
                return BadRequest(new { status = (int)ResponseType.ERROR, message = "On Execution", url_path = "" });
            }
        }
     
    }
}
