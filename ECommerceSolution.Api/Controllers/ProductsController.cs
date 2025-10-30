

using Application.Dtos.ProductsDtos;

using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSolution.Api.Controllers
{
    // API Route: /api/products
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        // DI ile IProductService'i alıyoruz
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

     

        /// <summary>
        /// Tüm ürünleri listeler.
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Herkes erişebilir
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            if (products == null || !products.Any())
            {
                return NoContent(); // 204
            }
            return Ok(products); // 200
        }

        /// <summary>
        /// ID'ye göre tek bir ürünü getirir.
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new
                {
                    Message = $"Ürün ID {id} bulunamadı.",
                    Status =404
                }); // 404
            }
            return Ok(product); // 200
        }


       
       

        /// <summary>
        /// Yeni bir ürün oluşturur. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolü erişebilir
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // JWT yok
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // JWT var ama rol Admin değil
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productDto = await _productService.CreateProductAsync(dto);

            // Başarılı oluşturma: 201 Created
            return CreatedAtAction(nameof(GetById), new { id = productDto.Id }, productDto);
        }

        /// <summary>
        /// Mevcut bir ürünü günceller. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _productService.UpdateProductAsync(id, dto);

            if (!success)
            {
                // Güncelleme başarısızsa (genellikle ID bulunamadığından)
                return NotFound();
            }

            return NoContent(); // 204 Success (RESTful prensip)
        }

        /// <summary>
        /// Bir ürünü siler. (Admin yetkisi gereklidir)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);

            if (!success)
            {
                return NotFound(new
                {
                    Message= $"Ürün ID {id} bulunamadı."
                });
            }

            return NoContent(); // 204 Success
        }
    }
}