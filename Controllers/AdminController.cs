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

        return View(dashboard);
    }
}