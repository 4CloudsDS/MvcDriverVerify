using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers;

public sealed class RelationshipsController : Controller
{
    private readonly DriverMarketplaceService _driverMarketplaceService;

    public RelationshipsController(DriverMarketplaceService driverMarketplaceService)
    {
        _driverMarketplaceService = driverMarketplaceService;
    }

    [Authorize(Policy = "OwnerWorkspace")]
    public IActionResult Index()
    {
        return View(new Models.DriverTrustDashboardViewModel());
    }

    [Authorize(Policy = "OwnerWorkspace")]
    [HttpGet]
    public async Task<IActionResult> Search(string query, string? mode, string? intent, string? relationshipType, CancellationToken cancellationToken)
    {
        var dashboard = await _driverMarketplaceService.SearchDashboardAsync(query, mode, intent, relationshipType, cancellationToken);

        return Json(dashboard);
    }
}
