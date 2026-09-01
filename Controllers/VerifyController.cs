using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Models;

namespace MvcDriverVerify.Controllers;

public sealed class VerifyController : Controller
{
    public IActionResult Index()
    {
        return View(new DriverTrustDashboardViewModel());
    }
}