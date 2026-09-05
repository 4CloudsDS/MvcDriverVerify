using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers;

[Authorize(Policy = "DriverWorkspace")]
public sealed class MeController : Controller
{
    private readonly DriverMarketplaceService _driverMarketplaceService;

    public MeController(DriverMarketplaceService driverMarketplaceService)
    {
        _driverMarketplaceService = driverMarketplaceService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await _driverMarketplaceService.GetMeDashboardAsync(cancellationToken);

        return View(dashboard);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRelationship(Guid caseId, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.UpdateVerificationCaseStatusAsync(caseId, "Verified", cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RequestReview(Guid caseId, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.UpdateVerificationCaseStatusAsync(caseId, "CounterpartyReview", cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DisputeRelationship(Guid caseId, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.UpdateVerificationCaseStatusAsync(caseId, "Disputed", cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }
}
