// ECommerceSolution.Core/Application/DTOs/UserCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        // Admin tarafından atanan rol
        [Required]
        public string Role { get; set; } // Örn: "Customer", "Admin"
    }
}