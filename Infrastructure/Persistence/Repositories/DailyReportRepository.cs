

using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence; 

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
   
    public class DailyReportRepository : GenericRepository<DailyReport>, IDailyReportRepository
    {
      
        public DailyReportRepository(AppDbContext context) : base(context)
        {
        }

        // Burası DailyReport'a özel implementasyonlar için ayrılmıştır.
    }
}