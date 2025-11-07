using Application.Dtos.UsersDtos;
using Application.Interfaces.Services;
using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        // private readonly IEmailService _emailService; // Opsiyonel: Hoş geldin maili için

        public UserManagementService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher /*, IEmailService emailService*/)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            // _emailService = emailService;
        }

        // --- CREATE ---
        public async Task<(bool Success, string Message, UserDto User)> CreateUserAsync(UserCreateDto createDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(createDto.Email);
            if (existingUser != null)
            {
                return (false, "Bu e-posta adresi zaten kullanımda.", null);
            }

            // Rol string olarak geliyorsa enum'a çevir
            if (!Enum.TryParse(createDto.Role, true, out UserRole roleEnum))
            {
                roleEnum = UserRole.Customer; // Geçersizse Customer olsun
            }

            
            var passwordHash = _passwordHasher.HashPassword(createDto.Password);

            var user = new User
            {
                Username = createDto.Username,
                Email = createDto.Email,
                PasswordHash = passwordHash,
                Role = roleEnum,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Kullanıcı başarıyla oluşturuldu.",
                new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString()
                });
        }

        // --- READ ---
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            }).ToList();
        }
        public async Task<UserDto?> GetUserByIdAsync(int userId) 
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        // --- UPDATE USER DETAILS ---
        public async Task<bool> UpdateUserDetailsAsync(int userId, UserUpdateDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Email != updateDto.Email)
            {
                var existing = await _userRepository.GetByEmailAsync(updateDto.Email);
                if (existing != null && existing.Id != userId)
                {
                    return false; // Email çakışması
                }
            }

            user.Username = updateDto.Username ?? user.Username;
            user.Email = updateDto.Email ?? user.Email;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // --- UPDATE ROLE ---
        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!Enum.TryParse(newRole, true, out UserRole roleEnum))
                return false;

            if (user.Role != roleEnum)
            {
                user.Role = roleEnum;
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // --- DELETE ---
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            _userRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
