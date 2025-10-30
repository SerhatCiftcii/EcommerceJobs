// ECommerceSolution.Api/Controllers/AuthController.cs

using Application.Dtos.UsersDtos;
using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Yeni bir kullanıcı (Müşteri) kaydı oluşturur.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _authService.RegisterAsync(model);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }

            return Ok(new { Message = message });
        }

        /// <summary>
        /// Kullanıcı girişi yapar ve JWT Token döndürür.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(new { Message = result.Message });
            }

            return Ok(result); // AuthResponseDto içerir
        }

        // Örnek: Giriş yapmış kullanıcının kendi bilgilerini görmesi (Sadece test için)
        [HttpGet("me")]
        [Authorize] // Herhangi bir yetkili kullanıcı erişebilir
        public async Task<IActionResult> GetCurrentUser()
        {
            // JWT'den gelen email claim'ini al
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "JWT token'da geçerli e-posta bilgisi yok." });
            }

            var userDto = await _authService.GetUserByEmailAsync(userEmail);

            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }
    }
}