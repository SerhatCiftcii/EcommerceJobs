

using Application.Dtos.UsersDtos;
using ECommerceSolution.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        // OLUŞTURMA
        Task<(bool Success, string Message, UserDto User)> CreateUserAsync(UserCreateDto createDto);

        // OKUMA
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);

        // GÜNCELLEME (Temel Bilgiler)
        Task<bool> UpdateUserDetailsAsync(int userId, UserUpdateDto updateDto);

        // ROL GÜNCELLEME
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);
        
        // SİLME
        Task<bool> DeleteUserAsync(int userId);
    }
}