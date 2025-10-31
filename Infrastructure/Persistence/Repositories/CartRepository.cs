

using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Cart> GetCartWithDetailsByUserIdAsync(int userId)
        {
            // Sepeti (Cart) UserId'ye göre bul ve detaylarını (CartItems ve Product) yükle
            return await _context.Carts
                                 .Where(c => c.UserId == userId)
                                 .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Product) // Product bilgisine erişmek için
                                 .FirstOrDefaultAsync();
        }

        public async Task<int?> GetCartIdByUserIdAsync(int userId)
        {
            // hızlı ID sorgusu
            var cartId = await _context.Carts
                                       .Where(c => c.UserId == userId)
                                       .Select(c => (int?)c.Id)
                                       .FirstOrDefaultAsync();
            return cartId;
        }
    }
}