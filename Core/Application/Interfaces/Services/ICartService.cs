

using Application.Dtos.OrdersDto;
using ECommerceSolution.Core.Application.DTOs;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface ICartService
    {
        // Sepeti getirir veya yoksa null döndürür
        Task<CartDto> GetCartAsync(int userId);

        // Sepete ürün ekler veya mevcut ürünün miktarını günceller
        Task<(bool Success, string Message, CartDto Cart)> AddOrUpdateItemAsync(int userId, CartItemManipulationDto itemDto);

        // Sepetten bir ürünü (CartItem Id ile) kaldırır
        Task<bool> RemoveItemAsync(int userId, int cartItemId);

        // Sepeti siparişe dönüştürür (Checkout)
        Task<(bool Success, string Message, OrderDto Order)> CheckoutAsync(int userId, string shippingAddress);
    }
}