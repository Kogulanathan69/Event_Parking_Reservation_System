using Event_Parking_Reservation_System.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Event_Parking_Reservation_System.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.Parse(_configuration["Smtp:Port"]!);
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var fromName = _configuration["Smtp:FromName"];

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!),
                Subject = "EventFlow Login OTP",
                Body = $"Your login OTP is: {otp}\n\nThis OTP expires in 5 minutes.",
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
