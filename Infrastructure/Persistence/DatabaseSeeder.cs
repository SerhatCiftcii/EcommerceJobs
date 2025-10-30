// ECommerceSolution.Infrastructure/Persistence/DatabaseSeeder.cs

using Application.Interfaces.Services;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Persistence
{
    public class DatabaseSeeder : IDatabaseSeeder // Yeni bir arayüz de tanımlayabiliriz
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        // Admin bilgileri sabit olarak burada tutulur
        private const string AdminEmail = "admin@ecommerce.com";
        private const string AdminPassword = "Admin123*";
        private const string AdminUsername = "SuperAdmin";

        public DatabaseSeeder(
            IGenericRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAdminUserAsync()
        {
            // Admin kullanıcısının var olup olmadığını kontrol et
            var existingUser = (await _userRepository.FindAsync(u => u.Email == AdminEmail)).FirstOrDefault();

            if (existingUser == null)
            {
                var passwordHash = _passwordHasher.HashPassword(AdminPassword);

                var adminUser = new User
                {
                    Username = AdminUsername,
                    Email = AdminEmail,
                    PasswordHash = passwordHash,
                    Role = UserRole.Admin
                };

                await _userRepository.AddAsync(adminUser);
                await _unitOfWork.SaveChangesAsync();

                // consolda görmek için yazdım. canlıya alırkan kaldırılabilir buda benım için dipnot.
                System.Console.WriteLine($"[SEED]: Admin kullanıcısı oluşturuldu: {AdminEmail}");
            }
            else
            {
                System.Console.WriteLine($"[SEED]: Admin kullanıcısı zaten mevcut: {AdminEmail}");
            }
        }
    }

    // DatabaseSeeder için basit bir arayüz
    public interface IDatabaseSeeder
    {
        Task SeedAdminUserAsync();
    }
}