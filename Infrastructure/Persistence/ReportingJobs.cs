// ECommerceSolution.Infrastructure/Jobs/ReportingJobs.cs (EKSİKSİZ VE DÜZELTİLMİŞ)

using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Jobs
{
    public class ReportingJobs
    {
        // GEREKLİ REPOSITORY VE UNIT OF WORK TANIMLARI
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDailyReportRepository _dailyReportRepository; // << DÜZELTME: Artık özel Repository'yi kullanıyoruz.
        private readonly IUnitOfWork _unitOfWork;

        // EKSİKSİZ VE GÜNCEL CONSTRUCTOR (TÜM BAĞIMLILIKLAR)
        public ReportingJobs(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IDailyReportRepository dailyReportRepository, // << CONSTRUCTOR PARAMETRESİ DÜZELTİLDİ
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _dailyReportRepository = dailyReportRepository; // << ATAMA DÜZELTİLDİ
            _unitOfWork = unitOfWork;
        }

        // ---------------------------------------------
        // GÜNLÜK RAPOR İŞİ (RECURRING JOB)
        // ---------------------------------------------
        public async Task CalculateDailyReport()
        {
            var targetDate = DateTime.UtcNow.Date.AddDays(-1); // Dün

            // 1. Düne ait siparişleri al
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(targetDate, targetDate);

            // 2. Düne ait yeni kayıt olan kullanıcıları al
            var newUsers = await _userRepository.GetUsersByRegistrationDateRangeAsync(targetDate, targetDate);

            // 3. Metrikleri Hesapla
            var totalSales = orders?.Sum(o => o.TotalAmount) ?? 0;
            var totalOrders = orders?.Count() ?? 0;
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
            var newUserCount = newUsers?.Count() ?? 0;

            // 4. Veritabanına kaydet (DailyReport tablosu)
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

            Console.WriteLine($"[Hangfire] Günlük Rapor Kaydedildi: {targetDate:yyyy-MM-dd}");
        }

        // ---------------------------------------------
        // DİĞER İŞLER
        // ---------------------------------------------
        public async Task CalculateMonthlySalesReport()
        {
            Console.WriteLine($"[Hangfire] Aylık Rapor İşlemi Başlatıldı: {DateTime.Now}");
            await Task.Delay(10);
        }

        public Task SyncStockQuantities()
        {
            Console.WriteLine($"[Hangfire] Stok Senkronizasyonu Başlatıldı: {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}