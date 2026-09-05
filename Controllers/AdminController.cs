using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers;

public sealed class AdminController : Controller
{
    private readonly DriverMarketplaceService _driverMarketplaceService;

    public AdminController(DriverMarketplaceService driverMarketplaceService)
    {
        _driverMarketplaceService = driverMarketplaceService;
    }

    [Authorize(Policy = "AdminWorkspace")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _driverMarketplaceService.GetAdminDashboardAsync("All drivers", cancellationToken));
    }

    [Authorize(Policy = "AdminWorkspace")]
    [HttpGet]
    public async Task<IActionResult> Dashboard(string? market, CancellationToken cancellationToken)
    {
        return Json(await _driverMarketplaceService.GetAdminDashboardAsync(market, cancellationToken));
    }
}