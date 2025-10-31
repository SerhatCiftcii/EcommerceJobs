// ECommerceSolution.Core/Domain/Entities/CartItem.cs

using System;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; } // 💡 Manuel olarak eklenen Id

        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 💡 Manuel olarak eklenen CreatedAt
        public DateTime? UpdatedAt { get; set; } // 💡 Manuel olarak eklenen UpdatedAt

        // Navigation Property: Ait olduğu Cart
        public Cart Cart { get; set; }

        // Navigation Property: Eklenen Ürün
        public Product Product { get; set; }
    }
}