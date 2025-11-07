using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")] // Sadece Admin erişebilir
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // 1. ANLIK ÖZET (Dashboard için)
    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    // 2. SON N GÜNLÜK DETAY (Grafikler için)
    // Örn: /api/reports/daily?days=30
    [HttpGet("daily")]
    public async Task<IActionResult> GetLastNDaysReports([FromQuery] int days = 7)
    {
        if (days <= 0 || days > 365)
        {
            return BadRequest("Gün sayısı 1 ile 365 arasında olmalıdır.");
        }
        var reports = await _reportService.GetLastNDaysReportsAsync(days);
        return Ok(reports);
    }

    // 3. AYLIK ÖZET RAPORU (Tablolar için)
    // Örn: /api/reports/monthly?year=2025&month=10
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        // Temel tarih kontrolü
        if (year < 2000 || month < 1 || month > 12)
        {
            return BadRequest("Geçersiz yıl veya ay formatı.");
        }

        var summary = await _reportService.GetMonthlySummaryAsync(year, month);

        if (summary == null)
        {
            return NotFound("Belirtilen ay ve yıla ait rapor bulunamadı.");
        }

        return Ok(summary);
    }
}