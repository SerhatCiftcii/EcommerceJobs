// ECommerceSolution.Infrastructure/Services/EmailService.cs

using ECommerceSolution.Core.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ECommerceSolution.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // --- Temel Gönderme Metodu ---
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // ... MimeMessage oluşturma ve SmtpClient ile gönderme kodu ...
            // (Kod Adım 51'deki ile aynıdır)

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["SmtpHost"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new BodyBuilder
                {
                    HtmlBody = isHtml ? body : null,
                    TextBody = isHtml ? null : body
                }.ToMessageBody();

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
                throw; // Hangfire'ın yeniden denemesi için hatayı fırlatıyoruz.
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        // --- Özel Email Metotları ---

        public Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            string subject = $"Hoş Geldiniz, {userName}!";
            string body = $"Merhaba {userName}, <p>E-ticaret platformumuza hoş geldiniz.</p>";
            return SendEmailAsync(toEmail, subject, body);
        }

        public Task SendOrderConfirmationEmailAsync(string toEmail, int orderId)
        {
            string subject = $"Siparişiniz Onaylandı: #{orderId}";
            string body = $"Merhaba, <p>#{orderId} numaralı siparişiniz başarıyla alınmıştır.</p>";
            return SendEmailAsync(toEmail, subject, body);
        }

        public Task SendCartReminderEmailAsync(string toEmail, string userName)
        {
            string subject = $"Sepetiniz Sizi Bekliyor, {userName}!";
            string body = $"Merhaba {userName}, <p>Sepetinizdeki harika ürünleri unutmuş olabilirsiniz.</p>";
            return SendEmailAsync(toEmail, subject, body);
        }
    }
}