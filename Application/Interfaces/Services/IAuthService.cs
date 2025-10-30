

using Application.Dtos.UsersDtos;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    // Kullanıcı yetkilendirme (kayıt/giriş) iş mantığı için sözleşme
    public interface IAuthService
    {
        // Kullanıcı Kaydı
        Task<(bool Success, string Message)> RegisterAsync(UserRegisterDto model);

        // Kullanıcı Girişi
        Task<AuthResponseDto> LoginAsync(UserLoginDto model);

        // Kullanıcıyı E-posta ile alma
        Task<UserDto> GetUserByEmailAsync(string email);
    }
}