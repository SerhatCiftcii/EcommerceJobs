

using ECommerceSolution.Core.Domain.Entities;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        
        /// Kullanıcı ID'sine göre sepeti, içindeki CartItems ve ilişkili Product bilgisiyle birlikte getirir.
        
        Task<Cart> GetCartWithDetailsByUserIdAsync(int userId);

        
        /// Kullanıcı ID'sine göre mevcut sepetin ID'sini getirir.
        
        Task<int?> GetCartIdByUserIdAsync(int userId);
    }
}