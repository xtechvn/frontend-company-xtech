using System.Net;
using System.Net.Mail;
using XTECH_FRONTEND.Models.Contact;

namespace XTECH_FRONTEND.Services
{
    public class EmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool sendMail(EmailContract model)
        {
            try
            {
                MailMessage message = new MailMessage();
                if (string.IsNullOrEmpty(model.Title))
                    model.Title = "Cảm ơn bạn đã quan tâm đến WEBSITE chúng tôi";
                message.Subject = model.Title;

                //config send email
                string from_mail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["FROM_MAIL"];
                  
                string account = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["USERNAME"];
                    
                string password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["PASSWORD"];
                    
                string host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["HOST"];
                    
                string port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["PORT"];
                string emailxtech = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["Email"];
                    
                message.IsBodyHtml = true;
                message.From = new MailAddress(from_mail);
                switch (model.type)
                {
                    case 1:
                        {
                            message.Body = GetTemplate(model);
                        }
                        break;
                    case 2:
                        {
                           
                        }
                        break;
                }
                
            
                SmtpClient smtp = new SmtpClient(host, Convert.ToInt32(port));
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(account, password);
                smtp.Timeout = 20000;
             
                
                message.To.Add("anhhieuk51@gmail.com");
                message.To.Add(emailxtech);
                smtp.Send(message);

                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
        public string GetTemplate(EmailContract model)
        {
            string workingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var template = workingDirectory + @"\MailTemplate\htmlEmail.html";
            string body = File.ReadAllText(template);
            body = body.Replace("{{Name}}", model.Name);
            body = body.Replace("{{Email}}", model.Email);
            body = body.Replace("{{ND}}", model.Note);
            return body;
        }
    }
}
