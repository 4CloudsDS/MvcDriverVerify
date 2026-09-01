using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers;

public sealed class ProfilesController : Controller
{
    private readonly DriverMarketplaceService _driverMarketplaceService;

    public ProfilesController(DriverMarketplaceService driverMarketplaceService)
    {
        _driverMarketplaceService = driverMarketplaceService;
    }

    public IActionResult Index()
    {
        return View(new Models.DriverTrustDashboardViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query, string? mode, string? intent, string? relationshipType, CancellationToken cancellationToken)
    {
        var dashboard = await _driverMarketplaceService.SearchDashboardAsync(query, mode, intent, relationshipType, cancellationToken);

        return Json(dashboard);
    }
}