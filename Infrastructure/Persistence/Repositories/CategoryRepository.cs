

using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence;

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        // ICategoryRepository'deki metotlar (şimdilik yok) GenericRepository'den gelir.
    }
}