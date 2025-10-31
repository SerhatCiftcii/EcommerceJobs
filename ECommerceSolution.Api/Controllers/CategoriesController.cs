

using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    // API Route: /api/categories
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        // DI ile ICategoryService'i alıyoruz
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        
        // MÜŞTERİ (Customer) ENDPOINT'leri - Yetkilendirme Gerekmez
       

        /// <summary>
        /// Tüm kategorileri listeler.
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Herkes erişebilir
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return NoContent(); // 204
            }
            return Ok(categories); // 200
        }

        /// <summary>
        /// ID'ye göre tek bir kategoriyi getirir.
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(); // 404
            }
            return Ok(category); // 200
        }

  
        // YÖNETİCİ (Admin) ENDPOINT'leri - Yetkilendirme Gerekir
      

        /// <summary>
        /// Yeni bir kategori oluşturur. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolü erişebilir
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryDto = await _categoryService.CreateCategoryAsync(dto);

            // Başarılı oluşturma: 201 Created
            return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id }, categoryDto);
        }

        /// <summary>
        /// Mevcut bir kategoriyi günceller. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _categoryService.UpdateCategoryAsync(id, dto);

            if (!success)
            {
                // Güncelleme başarısızsa (ID bulunamadığından)
                return NotFound();
            }

            return NoContent(); // 204 Success
        }

        /// <summary>
        /// Bir kategoriyi siler. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent(); // 204 Success
        }
    }
}