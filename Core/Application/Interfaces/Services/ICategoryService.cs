

using ECommerceSolution.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto createDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}