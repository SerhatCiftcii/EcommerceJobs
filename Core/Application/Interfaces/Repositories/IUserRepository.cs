
using ECommerceSolution.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System; // DateTime için

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
    // Temel CRUD'u GenericRepository'den alır
    public interface IUserRepository : IGenericRepository<User>
    {
        // AuthService ve UserManagement için: E-posta ile kullanıcı bulma
        Task<User> GetByEmailAsync(string email);

        // Admin'in tüm kullanıcıları listeleyebilmesi için metot
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Raporlama Job'ı için kritik metot: Tarih aralığında kayıt olan kullanıcıları çeker
        Task<IEnumerable<User>> GetUsersByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);
        
    }
}