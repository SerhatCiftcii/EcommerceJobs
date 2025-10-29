// ECommerceSolution.Core/Domain/Entities/Order.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Foreign Key: Kullanıcı ID
        [Required]
        public int UserId { get; set; }
        // Navigation property: Kullanıcı
        public User User { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Kargo adresi zorunludur.")]
        [MaxLength(500)]
        public string ShippingAddress { get; set; }

        public bool PaymentCompleted { get; set; } = false;

        // Navigation property: orderıtem
        public ICollection<OrderItem> OrderItems { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}