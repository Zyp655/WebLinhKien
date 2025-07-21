// File: Web-LinhKien/Services/EmailService.cs

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Web_LinhKien.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Thay đổi từ "EmailSettings" sang "SmtpSettings"
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["UserName"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
            };
            
            var mailMessage = new MailMessage
            {
                // Sử dụng "SenderEmail"
                From = new MailAddress(smtpSettings["SenderEmail"]), 
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}