// ECommerceSolution.Infrastructure/Services/AuthService.cs

using Application.Dtos.UsersDtos;
using Application.Interfaces.Services;

using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        // Tüm CRUD işlemleri için Generic Repository'i kullanıyoruz.
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        // IEmailService DI ile alınacak olsa da, şimdilik kullanılmayacak (Jobslar için planlandı)

        public AuthService(
            IGenericRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(UserRegisterDto model)
        {
            // 1. Kullanıcı zaten var mı kontrolü
            var existingUser = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();
            if (existingUser != null)
            {
                return (false, "Bu e-posta adresi zaten kullanımda.");
            }

            // 2. Şifreyi Hash'le
            var passwordHash = _passwordHasher.HashPassword(model.Password);

            // 3. Entity oluştur (Varsayılan rol: Customer)
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = UserRole.Customer
            };

            // 4. Kaydet
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Not: EmailService çağrısı veya Job tetikleme buraya gelecektir.

            return (true, "Kayıt başarılı.");
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto model)
        {
            // 1. Kullanıcıyı bul
            var user = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();

            if (user == null)
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Kullanıcı veya şifre hatalı." };
            }

            // 2. Şifreyi doğrula
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, model.Password))
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Kullanıcı veya şifre hatalı." };
            }

            // 3. JWT Token oluştur
            var token = _jwtService.GenerateToken(user);

            // 4. Başarılı yanıtı döndür
            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role.ToString(),
                Message = "Giriş başarılı."
            };
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = (await _userRepository.FindAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}