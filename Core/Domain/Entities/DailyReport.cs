

using System;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class DailyReport
    {
        public int Id { get; set; }

        // Raporun ait olduğu gün (Saat 00:00:00 olarak kaydedilmeli)
        public DateTime ReportDate { get; set; }

        // Hesaplanan Metrikler
      
        public decimal TotalSalesAmount { get; set; } // Günlük toplam satış (TL)
        public int TotalOrderCount { get; set; } // Günlük toplam sipariş adedi
        public decimal AverageOrderValue { get; set; } // Günlük ortalama sipariş değeri
        public int NewUserCount { get; set; } // O gün kayıt olan kullanıcı sayısı

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Raporun ne zaman oluşturulduğu
    }
}
