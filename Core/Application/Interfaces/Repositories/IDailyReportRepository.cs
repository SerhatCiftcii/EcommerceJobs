

using ECommerceSolution.Core.Domain.Entities;

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
 
    public interface IDailyReportRepository : IGenericRepository<DailyReport>
    {
       
        // Şimdilik sadece IGenericRepository'den miras alması yeterlidir.
    }
}