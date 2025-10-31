

using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        //  DTO Manual Mapping 
        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
          
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto createDto)
        {
            // DTO'dan Entity'ye Map
            var category = new Category
            {
                Name = createDto.Name
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = updateDto.Name;

            _categoryRepository.Update(category);
            var affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            _categoryRepository.Remove(category);
            var affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}