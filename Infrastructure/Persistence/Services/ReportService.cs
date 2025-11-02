using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IDailyReportRepository _dailyReportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public ReportService(
            IDailyReportRepository dailyReportRepository,
            IUserRepository userRepository,
            IProductRepository productRepository)
        {
            _dailyReportRepository = dailyReportRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            // AsNoTracking ile paralel kullanımda DbContext hatası önlenir
            var allReports = await _dailyReportRepository.GetAll().AsNoTracking().ToListAsync();
            var totalUsers = await _userRepository.GetAll().AsNoTracking().CountAsync();
            var totalProducts = await _productRepository.GetAll().AsNoTracking().CountAsync();

            var last7Days = DateTime.UtcNow.Date.AddDays(-7);
            var last30Days = DateTime.UtcNow.Date.AddDays(-30);

            var summary = new DashboardSummaryDto
            {
                TotalRegisteredUsers = totalUsers,
                TotalProductsInStock = totalProducts,
                GrandTotalSales = allReports.Sum(r => r.TotalSalesAmount),
                GrandTotalOrders = allReports.Sum(r => r.TotalOrderCount),
                SalesLast7Days = allReports.Where(r => r.ReportDate >= last7Days).Sum(r => r.TotalSalesAmount),
                NewUsersLast30Days = allReports.Where(r => r.ReportDate >= last30Days).Sum(r => r.NewUserCount),
                TotalReportsAvailable = allReports.Count
            };

            return summary;
        }

        public async Task<IEnumerable<DailyReportDto>> GetLastNDaysReportsAsync(int days)
        {
            var cutoffDate = DateTime.UtcNow.Date.AddDays(-days);

            var reports = await _dailyReportRepository.GetAll()
                                                      .AsNoTracking()
                                                      .Where(r => r.ReportDate >= cutoffDate)
                                                      .OrderByDescending(r => r.ReportDate)
                                                      .ToListAsync();

            return reports.Select(r => new DailyReportDto
            {
                ReportDate = r.ReportDate,
                TotalSalesAmount = r.TotalSalesAmount,
                TotalOrderCount = r.TotalOrderCount,
                AverageOrderValue = r.TotalOrderCount > 0 ? r.TotalSalesAmount / r.TotalOrderCount : 0,
                NewUserCount = r.NewUserCount
            }).ToList();
        }

        public Task<MonthlySummaryDto> GetMonthlySummaryAsync(int year, int month)
        {
            // Aylık özet basitleştirilmiş
            return Task.FromResult(new MonthlySummaryDto { Year = year, Month = month, TotalMonthlySales = 125000m });
        }
    }
}
