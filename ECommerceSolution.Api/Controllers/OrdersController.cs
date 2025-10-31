

using Application.Dtos.OrdersDto;

using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // --- Yardımcı Metot ---
        // JWT Claim'inden kullanıcı ID'sini güvenli bir şekilde alır
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            // Yetkili bir kullanıcının buraya ulaşmaması gerekir,
            // ancak hata durumunda 0 döndürerek servisin başarısız olmasını sağladım.
            return 0;
        }

      
        // MÜŞTERİ (Customer) ENDPOINT'leri
       

        /// <summary>
        /// Giriş yapan kullanıcı adına yeni bir sipariş oluşturur.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")] // Sadece müşteri rolü sipariş verebilir
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            if (userId == 0) return Unauthorized(new { Message = "Geçerli kullanıcı kimliği alınamadı." });

            var (success, message, orderDto) = await _orderService.CreateOrderAsync(userId, createDto);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }

            // Başarılı oluşturma: 201 Created
            return CreatedAtAction(nameof(GetUserOrderDetails), new { id = orderDto.Id }, orderDto);
        }

        /// <summary>
        /// Giriş yapan kullanıcının tüm siparişlerini listeler.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized(new { Message = "Geçerli kullanıcı kimliği alınamadı." });

            var orders = await _orderService.GetUserOrdersAsync(userId);

            if (orders == null || !orders.Any())
            {
                return NoContent(); // 204
            }
            return Ok(orders); // 200
        }

        /// <summary>
        /// Kullanıcının tek bir siparişinin detaylarını getirir.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserOrderDetails(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized(new { Message = "Geçerli kullanıcı kimliği alınamadı." });

            // Servis içinde hem siparişin varlığı hem de kullanıcıya ait olup olmadığı kontrol edilir
            var order = await _orderService.GetOrderDetailsAsync(id, userId);

            if (order == null)
            {
                // Eğer sipariş yoksa veya kullanıcıya ait değilse 404 döndürmek güvenlik açısından daha iyidir.
                return NotFound();
            }
            return Ok(order);
        }

        
        // YÖNETİCİ (Admin) ENDPOINT'leri
        

        /// <summary>
        /// Tüm sistemdeki siparişleri listeler. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            if (orders == null || !orders.Any())
            {
                return NoContent();
            }
            return Ok(orders);
        }

        /// <summary>
        /// Bir siparişin durumunu günceller. (Admin yetkisi gereklidir)
        /// </summary>
        // PUT'ta gövde (body) yerine, route üzerinden yeni statüyü almak daha temiz.
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                return BadRequest(new { Message = "Yeni sipariş durumu belirtilmelidir." });
            }

            var success = await _orderService.UpdateOrderStatusAsync(id, newStatus);

            if (!success)
            {
                // Sipariş bulunamadı
                return NotFound();
            }

            return NoContent(); // 204
        }
    }
}