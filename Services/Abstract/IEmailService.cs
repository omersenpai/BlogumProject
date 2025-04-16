using System.Threading.Tasks;

namespace BlogProject.Services.Abstract
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
} 