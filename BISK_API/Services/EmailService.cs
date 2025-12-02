using GardeningAPI.Application.Interfaces;
//using gardnerAPIs.Data;
using GardeningAPI.Data;
using System.Net;
using System.Net.Mail;

namespace GardeningAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly string? _mail;
        private readonly string? _password;
        public EmailService() 
        {
            var config = ConfigManager.Instance.getConfig();
            _mail = config["EmailCredentials:Email"];
            _password = config["EmailCredentials:Password"];
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(_mail) || string.IsNullOrEmpty(_password))
            {
                throw new InvalidOperationException("Email credentials are not configured properly.");
            }

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_mail, _password)
            };


            var mailMessage = new MailMessage(_mail, email, subject, message);
            return client.SendMailAsync(mailMessage);
        }
    }
}
