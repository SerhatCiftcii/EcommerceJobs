

namespace ECommerceSolution.Core.Application.DTOs
{
    public class DashboardSummaryDto
    {
        // 1. Genel Metrikler (Tüm zamanlar)
        public int TotalRegisteredUsers { get; set; }
        public int TotalProductsInStock { get; set; }

        // 2. Kümülatif Metrikler (Tüm zamanlar)
        public decimal GrandTotalSales { get; set; } // Tüm zamanların toplam satışı
        public int GrandTotalOrders { get; set; }

        // 3. Yakın Geçmiş Metrikleri (DailyReport'tan çekilir)
        public decimal SalesLast7Days { get; set; }
        public int NewUsersLast30Days { get; set; }
        public int TotalReportsAvailable { get; set; } // Kaç günlük rapor olduğu
    }
}