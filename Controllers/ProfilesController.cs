using Microsoft.AspNetCore.Mvc;

namespace MvcDriverVerify.Controllers;

public sealed class ProfilesController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Relationships");
    }

    [HttpGet]
    public IActionResult Search(string query, string? mode, string? intent, string? relationshipType)
    {
        return RedirectToAction("Search", "Relationships", new { query, mode, intent, relationshipType });
    }
}
