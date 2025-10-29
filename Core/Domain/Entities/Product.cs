// ECommerceSolution.Core/Domain/Entities/Product.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [MaxLength(200)]
        public string Name { get; set; }

        // Finansal veriler için kesinlik tanımı (SQL Server/PostgreSQL vb. için önerilir)
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [MaxLength(500)]
        // Resim URL'si zorunlu değil
        public string ImageUrl { get; set; }

        // Foreign Key: Kategori ID
        [Required]
        public int CategoryId { get; set; }
        // Navigation property: Kategori
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}