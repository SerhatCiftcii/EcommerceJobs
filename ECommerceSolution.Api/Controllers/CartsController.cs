

using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    // API Route: /api/carts
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")] 
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // --- Yardımcı Metot ---
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0; // Yetkili kullanıcıda 0 dönmemeli, ama güvenlik için kontrol
        }

       
        // SEPET İŞLEMLERİ (CRUD)
       

        /// <summary>
        /// Giriş yapan kullanıcının mevcut sepetini getirir.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
            {
                // Sepet boşsa veya hiç oluşturulmamışsa 404 (veya 200 boş sepet) döndürülebilir.
                // Biz burada 200 OK ve boş bir liste döndürmeyi tercih edelim.
                return Ok(new CartDto
                {
                    UserId = userId,
                    Items = new List<CartItemDto>()
                });
            }

            return Ok(cart);
        }

        /// <summary>
        /// Sepete ürün ekler veya mevcut ürünün miktarını günceller.
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddOrUpdateItem([FromBody] CartItemManipulationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var (success, message, cartDto) = await _cartService.AddOrUpdateItemAsync(userId, dto);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }

            return Ok(cartDto); // Güncel sepeti döndür
        }

        /// <summary>
        /// Sepetten belirli bir ürünü (CartItem Id ile) kaldırır.
        /// </summary>
        [HttpDelete("items/{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userId = GetUserId();
            var success = await _cartService.RemoveItemAsync(userId, cartItemId);

            if (!success)
            {
                return NotFound(); // Sepet ya da ürün bulunamadı
            }

            return NoContent(); // 204 Success
        }

    
        // SİPARİŞ VERME (CHECKOUT)
       

        /// <summary>
        /// Sepeti siparişe dönüştürür (Checkout). Sepet temizlenir ve Order oluşturulur.
        /// </summary>
        [HttpPost("checkout")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkout([FromQuery] string shippingAddress)
        {
            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                return BadRequest(new { Message = "Gönderim adresi zorunludur." });
            }

            var userId = GetUserId();
            var (success, message, orderDto) = await _cartService.CheckoutAsync(userId, shippingAddress);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }

            // Başarılı sipariş: 201 Created (Oluşturulan siparişin detayını döndürürüz)
            return CreatedAtAction("GetUserOrderDetails", "Orders", new { id = orderDto.Id }, orderDto);
        }
    }
}