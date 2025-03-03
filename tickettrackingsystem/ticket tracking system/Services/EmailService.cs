using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ticket_tracking_system.Models;

namespace ticket_tracking_system.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailConfiguration _emailConfig;

        public EmailService(IOptions<EmailConfiguration> emailConfig)
        {
            _emailConfig = emailConfig.Value;
            _smtpClient = new SmtpClient(_emailConfig.SmtpServer)
            {
                Port = _emailConfig.Port,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password),
                EnableSsl = true,
                Timeout = 300000
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailConfig.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}

