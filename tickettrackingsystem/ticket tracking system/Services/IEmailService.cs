using System.Threading.Tasks;

namespace ticket_tracking_system.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}

