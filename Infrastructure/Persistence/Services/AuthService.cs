// ECommerceSolution.Infrastructure/Services/AuthService.cs

using Application.Dtos.UsersDtos;
using Application.Interfaces.Services;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using Hangfire; // << GEREKLİ: Hangfire Client için
using System.Linq;
using System.Threading.Tasks;

// Not: using Application.Interfaces.Services; satırı gereksizdir ve kaldırılmıştır, 
// çünkü IAuthService zaten ECommerceSolution.Core.Application.Interfaces.Services altındadır.

namespace ECommerceSolution.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IBackgroundJobClient _jobClient; // << YENİ: Hangfire Job Tetikleyicisi

        // EKSİKSİZ CONSTRUCTOR
        public AuthService(
            IGenericRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IBackgroundJobClient jobClient) // << CONSTRUCTOR'A EKLENDİ
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _jobClient = jobClient; // << ATAMA YAPILDI
        }

        public async Task<(bool Success, string Message)> RegisterAsync(UserRegisterDto model)
        {
            // 1. Kullanıcı zaten var mı kontrolü
            var existingUser = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();
            if (existingUser != null)
            {
                return (false, "Bu e-posta adresi zaten kullanımda.");
            }

            // 2. Şifreyi Hash'le ve Entity oluştur
            var passwordHash = _passwordHasher.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = UserRole.Customer,
               // CreationDate = DateTime.UtcNow // Yeni kullanıcı raporlama için tarih de ekleyelim
            };

            // 3. Kaydet
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 4. >> YENİ KOD: Hoş Geldin E-postası Job'ını Tetikle (Fire-and-Forget) <<
            // Bu, hemen arka planda asenkron olarak çalışacaktır.
            _jobClient.Enqueue<IEmailService>(service => service.SendWelcomeEmailAsync(user.Email, user.Username));

            return (true, "Kayıt başarılı. Hoş geldin e-postası gönderiliyor...");
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto model)
        {
            var user = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, model.Password))
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "Kullanıcı veya şifre hatalı." };
            }

            var token = _jwtService.GenerateToken(user);

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