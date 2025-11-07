using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Jobs
{
    public class ReportingJobs
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDailyReportRepository _dailyReportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReportingJobs(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IDailyReportRepository dailyReportRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _dailyReportRepository = dailyReportRepository;
            _unitOfWork = unitOfWork;
        }

        // -----------------------------
        // Günlük rapor
        // -----------------------------
        public async Task CalculateDailyReport()
        {
            var targetDate = DateTime.UtcNow.Date.AddDays(-1); // Dün

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(targetDate, targetDate);
            var newUsers = await _userRepository.GetUsersByRegistrationDateRangeAsync(targetDate, targetDate);

            var totalSales = orders?.Sum(o => o.TotalAmount) ?? 0;
            var totalOrders = orders?.Count() ?? 0;
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
            var newUserCount = newUsers?.Count() ?? 0;

            var existingReport = await _dailyReportRepository.GetAll()
                .Where(r => r.ReportDate == targetDate)
                .FirstOrDefaultAsync();

            if (existingReport == null)
            {
                var report = new DailyReport
                {
                    ReportDate = targetDate,
                    TotalSalesAmount = totalSales,
                    TotalOrderCount = totalOrders,
                    AverageOrderValue = averageOrderValue,
                    NewUserCount = newUserCount
                };

                await _dailyReportRepository.AddAsync(report);
                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"[Hangfire] Günlük rapor kaydedildi: {targetDate:yyyy-MM-dd}");
            }
            else
            {
                existingReport.TotalSalesAmount = totalSales;
                existingReport.TotalOrderCount = totalOrders;
                existingReport.AverageOrderValue = averageOrderValue;
                existingReport.NewUserCount = newUserCount;

                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine($"[Hangfire] Günlük rapor güncellendi: {targetDate:yyyy-MM-dd}");
            }
        }

        // -----------------------------
        // Aylık rapor (async + parametreli)
        // -----------------------------
        public async Task CalculateMonthlyReport(int? year = null, int? month = null)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var startDate = new DateTime(targetYear, targetMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var reports = await _dailyReportRepository.GetAll()
                .Where(r => r.ReportDate >= startDate && r.ReportDate <= endDate)
                .ToListAsync();

            if (!reports.Any())
            {
                Console.WriteLine($"[Hangfire] {targetYear}-{targetMonth} için günlük veri yok.");
                return;
            }

            var totalSales = reports.Sum(r => r.TotalSalesAmount);
            var totalOrders = reports.Sum(r => r.TotalOrderCount);
            var totalNewUsers = reports.Sum(r => r.NewUserCount);

            Console.WriteLine($"[Hangfire] {targetYear}-{targetMonth} Aylık Toplam Satış: {totalSales}, Sipariş: {totalOrders}, Yeni Kullanıcı: {totalNewUsers}");
        }

        // -----------------------------
        // Parametresiz overload (Hangfire için)
        // -----------------------------
        public Task CalculateMonthlyReport()
        {
            return CalculateMonthlyReport(null, null);
        }

        // -----------------------------
        // Stok senkronizasyonu
        // -----------------------------
        public Task SyncStockQuantities()
        {
            Console.WriteLine($"[Hangfire] Stok senkronizasyonu başlatıldı: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
