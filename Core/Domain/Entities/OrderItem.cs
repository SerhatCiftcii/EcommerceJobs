

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Foreign Key
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Foreign Key
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Satın alındığı anki fiyatın korunması
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPriceAtPurchase { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}