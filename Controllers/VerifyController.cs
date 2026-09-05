using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Models;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers;

public sealed class VerifyController : Controller
{
    private readonly DriverMarketplaceService _driverMarketplaceService;

    public VerifyController(DriverMarketplaceService driverMarketplaceService)
    {
        _driverMarketplaceService = driverMarketplaceService;
    }

    public IActionResult Index()
    {
        return View(new DriverTrustDashboardViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VerificationCaseSubmission request, CancellationToken cancellationToken)
    {
        var result = await _driverMarketplaceService.CreateVerificationCaseAsync(request, cancellationToken);

        return result.Accepted ? Created(string.Empty, result) : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}