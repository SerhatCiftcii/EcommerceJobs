// ECommerceSolution.Core/Application/DTOs/CartItemDto.cs

namespace ECommerceSolution.Core.Application.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; } // Sepet ıtemın kendi ID'si
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}