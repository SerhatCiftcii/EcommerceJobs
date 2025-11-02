// ECommerceSolution.Core/Application/Interfaces/Services/IReportService.cs

using ECommerceSolution.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Admin Dashboard'u için genel özet metriklerini (Kümülatif satış, kullanıcı sayısı vb.) çeker.
        /// </summary>
        /// <returns>DashboardSummaryDto nesnesi.</returns>
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        /// <summary>
        /// Son N güne ait günlük rapor verilerini (grafikler ve detay tabloları için) çeker.
        /// </summary>
        /// <param name="days">Geriye dönük çekilecek gün sayısı.</param>
        /// <returns>DailyReportDto listesi.</returns>
        Task<IEnumerable<DailyReportDto>> GetLastNDaysReportsAsync(int days);

        /// <summary>
        /// Belirli bir yıl ve aya ait aylık rapor özetini DailyReport verilerinden hesaplar.
        /// </summary>
        /// <param name="year">İstenen yıl.</param>
        /// <param name="month">İstenen ay.</param>
        /// <returns>MonthlySummaryDto nesnesi.</returns>
        Task<MonthlySummaryDto> GetMonthlySummaryAsync(int year, int month);
    }
}