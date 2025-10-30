


using Application.Dtos.ProductsDtos;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface IProductService
    {
        // Müşteri Tarafı Metotlar
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);

        // Yönetici Tarafı (CRUD) Metotlar
        Task<ProductDto> CreateProductAsync(ProductCreateDto createDto);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDto updateDto);
        Task<bool> DeleteProductAsync(int id);

        // Ek Metot
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    }
}