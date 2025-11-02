

using ECommerceSolution.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")] 
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("summary")] 
    public async Task<IActionResult> GetDashboardSummary()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    // ... Diğer Raporlama Metotları (GetLastNDaysReports, GetMonthlySummary) sonra eklicem.
}