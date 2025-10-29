

using System;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces
{
    public interface IUnitOfWork
    {
        // Temel işlem: Bağlamdaki değişiklikleri veritabanına kaydet
        Task<int> SaveChangesAsync();
    }
}