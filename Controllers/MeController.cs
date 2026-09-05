using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Models;
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
    public async Task<IActionResult> UpdateProfile(ProfileUpdateSubmission request, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.UpdateProfileAsync(request, cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SaveVehicle(VehicleUpdateSubmission request, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.AddOrUpdateVehicleAsync(request, cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRelationship(RelationshipUpdateSubmission request, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.UpdateRelationshipAsync(request, cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRelationship(string relationshipId, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.DeleteRelationshipAsync(relationshipId, cancellationToken);
        TempData["MeStatus"] = result.Message;

        return RedirectToAction(nameof(Index));
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
