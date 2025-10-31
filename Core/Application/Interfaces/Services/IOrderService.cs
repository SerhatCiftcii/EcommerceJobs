// ECommerceSolution.Core/Application/Interfaces/Services/IOrderService.cs

using Application.Dtos.OrdersDto;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface IOrderService
    {
        // Kullanıcı için yeni bir sipariş oluşturma
        // userId: JWT token'dan alınacak.
        Task<(bool Success, string Message, OrderDto Order)> CreateOrderAsync(int userId, OrderCreateDto createDto);

        // Kullanıcının kendi siparişlerini listelemesi
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);

        // Kullanıcının tek bir siparişinin detaylarını görmesi
        Task<OrderDto> GetOrderDetailsAsync(int orderId, int userId);

        // YÖNETİCİ: Tüm siparişleri listeleme (Raporlama amaçlı)
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        // YÖNETİCİ: Sipariş durumunu güncelleme (Örn: Pending -> Shipped)
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}