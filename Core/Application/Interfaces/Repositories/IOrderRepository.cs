

using ECommerceSolution.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
  
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Kullanıcının tüm siparişlerini OrderItems ve Product bilgisi ile birlikte getirir.
        Task<IEnumerable<Order>> GetOrdersWithDetailsByUserIdAsync(int userId);

        // Yönetici için tüm siparişleri detaylarla birlikte getirir (Örn: Raporlama)
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();

        // Siparişi, OrderItems ve Product nesneleriyle birlikte ID'ye göre getirir.
        Task<Order> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}