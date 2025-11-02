

using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        // Temel gönderme metodu
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);

        // Hoş Geldin Maili (Hangfire Fire-and-Forget için)
        Task SendWelcomeEmailAsync(string toEmail, string userName);

        // Sipariş Onay Maili (Hangfire Fire-and-Forget için)
        Task SendOrderConfirmationEmailAsync(string toEmail, int orderId);

        // Sepet Hatırlatma Maili (Hangfire Delayed Job için - Daha sonra detaylandırılacak)
        Task SendCartReminderEmailAsync(string toEmail, string userName);
    }
}