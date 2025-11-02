

using System;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        // Aylık Toplamlar
        public decimal TotalMonthlySales { get; set; }
        public int TotalMonthlyOrders { get; set; }
        public int TotalNewUsers { get; set; }

        // Opsiyonel: Büyüme Oranı
        public decimal SalesGrowthRate { get; set; } = 0;
    }
}
