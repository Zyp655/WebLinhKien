// File: Web-LinhKien/Services/IEmailService.cs

using System.Threading.Tasks;

namespace Web_LinhKien.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}