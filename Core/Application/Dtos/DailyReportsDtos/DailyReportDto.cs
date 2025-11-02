

using System;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class DailyReportDto
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public int TotalOrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int NewUserCount { get; set; }
    }
}