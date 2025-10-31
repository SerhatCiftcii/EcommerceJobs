// ECommerceSolution.Core/Application/DTOs/CartDto.cs

using System.Collections.Generic;
using System.Linq;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }

        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        // Sadece DTO içinde hesaplanan alan
        public decimal TotalAmount => Items.Sum(i => i.LineTotal);

        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }
}