using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // 🔹 Kullanıcının siparişlerini tüm detaylarla birlikte getir
        public async Task<IEnumerable<Order>> GetOrdersWithDetailsByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.User) // 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // 🔹 Yönetici için tüm siparişleri getir
        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.User) // 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // 🔹 ID'ye göre tek siparişi getir
        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User) // 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // 🔹 Belirli tarih aralığındaki siparişleri getir
        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.User) // 
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
