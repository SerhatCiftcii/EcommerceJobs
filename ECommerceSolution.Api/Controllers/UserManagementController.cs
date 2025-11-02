

using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    // API Route: /api/admin/usermanagement
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

    

        /// <summary>
        /// Admin tarafından yeni bir kullanıcı (Müşteri veya Admin) oluşturur.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, userDto) = await _userManagementService.CreateUserAsync(createDto);

            if (!success)
            {
                return BadRequest(new { Message = message }); // E-posta çakışması veya geçersiz rol
            }

            // Başarılı kullanıcı oluşturma (Hoş geldin maili bu aşamada Hangfire'a atılabilir.)
            return CreatedAtAction(nameof(GetAllUsers), new { id = userDto.Id }, userDto);
        }

        // ------------------------------------------------------------------
        // 2. OKUMA (READ)
        // ------------------------------------------------------------------

        /// <summary>
        /// Sistemdeki tüm kullanıcıları listeler.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return Ok(users);
        }

   

        /// <summary>
        /// Bir kullanıcının kullanıcı adı ve e-posta gibi temel bilgilerini günceller.
        /// </summary>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserDetails(int userId, [FromBody] UserUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _userManagementService.UpdateUserDetailsAsync(userId, updateDto);

            if (!success)
            {
                return BadRequest(new { Message = "Kullanıcı detayları güncellenemedi. E-posta adresi zaten kullanımda olabilir veya kullanıcı bulunamadı." });
            }

            return NoContent(); // 204 No Content: Güncelleme başarılı
        }

      

        /// <summary>
        /// Bir kullanıcının rolünü günceller (Örn: Customer -> Admin).
        /// </summary>
        [HttpPut("{userId}/role/{newRole}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            var success = await _userManagementService.UpdateUserRoleAsync(userId, newRole);

            if (!success)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı veya rol geçersiz." });
            }

            return NoContent();
        }

     

      
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var success = await _userManagementService.DeleteUserAsync(userId);

            if (!success)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            return NoContent();
        }
    }
}