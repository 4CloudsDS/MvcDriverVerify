using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcDriverVerify.Models;
using MvcDriverVerify.Services;

namespace MvcDriverVerify.Controllers
{
    public class HomeController : Controller
    {
        private readonly DriverMarketplaceService _driverMarketplaceService;

        public HomeController(DriverMarketplaceService driverMarketplaceService)
        {
            _driverMarketplaceService = driverMarketplaceService;
        }

        public IActionResult Index()
        {
            return View(new DriverTrustDashboardViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string query, CancellationToken cancellationToken)
        {
            var dashboard = await _driverMarketplaceService.SearchDashboardAsync(query, cancellationToken);

            return Json(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
