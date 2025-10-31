

using ECommerceSolution.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
    // Category'ye özel metotlar eklenme ihtimaline karşı tanımlanır
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Şimdilik sadece Generic Repository'deki metotları miras alır. ielrde özel metoteklenebeilir.
        
    }
}