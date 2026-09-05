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

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await _driverMarketplaceService.GetDashboardAsync(cancellationToken);
        var moderationQueue = await _driverMarketplaceService.GetModerationQueueAsync(cancellationToken);

        return View(new Models.DriverTrustDashboardViewModel
        {
            ApiConnected = dashboard.ApiConnected,
            ApiStatus = dashboard.ApiStatus,
            Drivers = dashboard.Drivers,
            ModerationQueue = moderationQueue
        });
    }
}