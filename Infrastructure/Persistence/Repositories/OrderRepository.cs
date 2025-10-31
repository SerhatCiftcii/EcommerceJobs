

using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        

        public async Task<IEnumerable<Order>> GetOrdersWithDetailsByUserIdAsync(int userId)
        {
            return await _context.Orders
                                 .Where(o => o.UserId == userId)
                                 // OrderItems ve ilişkili Product bilgisini dahil et
                                 .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                                 // OrderItems ve ilişkili Product bilgisini dahil et
                                 .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                 .Include(o => o.User) // Kullanıcı bilgisini de ekleyebiliriz
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }

        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                                 .Where(o => o.Id == orderId)
                                 // OrderItems ve ilişkili Product bilgisini dahil et
                                 .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                 .FirstOrDefaultAsync();
        }
    }
}