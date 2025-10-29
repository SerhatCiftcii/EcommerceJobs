
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        // Şifre hash'i tutulacak
        public string PasswordHash { get; set; }

        // Enum'lar için HasConversion ayarı AppDbContext'te yapılacak
        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }

    public enum UserRole
    {
        Customer,
        Admin
    }
}