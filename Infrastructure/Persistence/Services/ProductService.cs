

using Application.Dtos.ProductsDtos;

using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository; // Kategori verisi için
        private readonly IUnitOfWork _unitOfWork;

       
        public ProductService(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            IGenericRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        // --- Müşteri Tarafı ---
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Category bilgisini de yükleyerek ürünleri al
            var products = await _productRepository.GetProductsWithCategoryAsync();

            // Entity'den DTO'ya Manuel Dönüşüm
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            }).ToList();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            // Entity'den DTO'ya Dönüşüm
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }

        // --- Yönetici Tarafı (CRUD) ---
        public async Task<ProductDto> CreateProductAsync(ProductCreateDto createDto)
        {
            // DTO'dan Entity'ye Map
            var product = new Product
            {
                Name = createDto.Name,
                Price = createDto.Price,
                StockQuantity = createDto.StockQuantity,
                ImageUrl = createDto.ImageUrl,
                CategoryId = createDto.CategoryId
            };

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(); // Kaydetme

            // Başarılıysa DTO olarak döndür
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDto updateDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            // Entity'yi güncelle
            product.Name = updateDto.Name;
            product.Price = updateDto.Price;
            product.StockQuantity = updateDto.StockQuantity;
            product.ImageUrl = updateDto.ImageUrl;
            product.CategoryId = updateDto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            var affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            _productRepository.Remove(product);
            var affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }

        // --- Ek Metot ---
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.FindAsync(p => p.CategoryId == categoryId);

            // Mapleme
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId
            }).ToList();
        }
    }
}