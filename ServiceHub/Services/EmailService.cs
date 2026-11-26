using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ServiceHub.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string toName, string serviceName, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        Task SendBookingStatusUpdateAsync(string toEmail, string toName, string serviceName, string status);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string toName, string serviceName,
            DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            var subject = $"✅ Booking Confirmed - {serviceName}";
            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Inter, sans-serif; background: #F8FAFC; padding: 20px;'>
  <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.1);'>
    <div style='background: linear-gradient(135deg, #4F46E5, #3B82F6); padding: 32px; text-align: center;'>
      <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Booking Confirmed!</h1>
    </div>
    <div style='padding: 32px;'>
      <p style='color: #374151; font-size: 16px;'>Hi <strong>{toName}</strong>,</p>
      <p style='color: #374151;'>Your booking has been successfully placed on <strong>ServiceHub</strong>.</p>
      <div style='background: #F0F9FF; border-left: 4px solid #4F46E5; padding: 16px; border-radius: 8px; margin: 24px 0;'>
        <p style='margin: 0; color: #1E40AF;'><strong>Service:</strong> {serviceName}</p>
        <p style='margin: 8px 0 0; color: #1E40AF;'><strong>Date:</strong> {bookingDate:dddd, MMMM dd, yyyy}</p>
        <p style='margin: 8px 0 0; color: #1E40AF;'><strong>Time:</strong> {DateTime.Today.Add(startTime):h:mm tt} – {DateTime.Today.Add(endTime):h:mm tt}</p>
      </div>
      <p style='color: #6B7280; font-size: 14px;'>The service provider will confirm your booking shortly. You'll receive another notification once confirmed.</p>
      <a href='#' style='display: inline-block; background: #4F46E5; color: white; padding: 12px 28px; border-radius: 8px; text-decoration: none; font-weight: 600; margin-top: 16px;'>View Booking Details</a>
    </div>
    <div style='background: #F8FAFC; padding: 20px; text-align: center;'>
      <p style='color: #9CA3AF; font-size: 12px; margin: 0;'>© 2025 ServiceHub. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";
            await SendEmailAsync(toEmail, toName, subject, body);
        }

        public async Task SendBookingStatusUpdateAsync(string toEmail, string toName, string serviceName, string status)
        {
            var emoji = status == "Accepted" ? "✅" : status == "Rejected" ? "❌" : "🎯";
            var subject = $"{emoji} Booking {status} - {serviceName}";
            var color = status == "Accepted" ? "#10B981" : status == "Rejected" ? "#EF4444" : "#4F46E5";
            var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Inter, sans-serif; background: #F8FAFC; padding: 20px;'>
  <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.1);'>
    <div style='background: {color}; padding: 32px; text-align: center;'>
      <h1 style='color: white; margin: 0;'>{emoji} Booking {status}</h1>
    </div>
    <div style='padding: 32px;'>
      <p style='color: #374151;'>Hi <strong>{toName}</strong>,</p>
      <p style='color: #374151;'>Your booking for <strong>{serviceName}</strong> has been <strong>{status}</strong>.</p>
      <a href='#' style='display: inline-block; background: {color}; color: white; padding: 12px 28px; border-radius: 8px; text-decoration: none; font-weight: 600;'>View Dashboard</a>
    </div>
    <div style='background: #F8FAFC; padding: 20px; text-align: center;'>
      <p style='color: #9CA3AF; font-size: 12px; margin: 0;'>© 2025 ServiceHub. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";
            await SendEmailAsync(toEmail, toName, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var smtpHost = _config["Email:SmtpHost"] ?? "localhost";
                var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var smtpUser = _config["Email:SmtpUser"] ?? "";
                var smtpPass = _config["Email:SmtpPass"] ?? "";
                var fromEmail = _config["Email:FromEmail"] ?? "noreply@servicehub.com";
                var fromName = _config["Email:FromName"] ?? "ServiceHub";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                if (!string.IsNullOrEmpty(smtpUser))
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email sending failed (non-critical): {Error}", ex.Message);
                // Email failures are non-critical - log and continue
            }
        }
    }
}
