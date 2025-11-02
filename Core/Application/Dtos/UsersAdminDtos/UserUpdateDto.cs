// ECommerceSolution.Core/Application/DTOs/UserUpdateDto.cs

using System.ComponentModel.DataAnnotations;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class UserUpdateDto
    {
        // Password veya Role haricindeki temel bilgileri günceller
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}