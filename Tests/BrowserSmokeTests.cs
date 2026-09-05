using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MvcDriverVerify.Tests;

public sealed class BrowserSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BrowserSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Relationships")]
    [InlineData("/Verify")]
    [InlineData("/Me")]
    [InlineData("/Admin")]
    public async Task Core_pages_render_successfully(string path)
    {
        using var response = await _client.GetAsync(path);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Verify Driver", html);
    }

    [Fact]
    public async Task Landing_and_relationship_search_endpoints_are_repeatable_smoke_checks()
    {
        using var landingSearch = await _client.GetAsync("/Home/Verify?query=Thabo");
        using var relationshipSearch = await _client.GetAsync("/Relationships/Search?query=Delivery&mode=opportunity&intent=looking-for-driver&relationshipType=Employment");
        var landingJson = await landingSearch.Content.ReadAsStringAsync();
        var relationshipJson = await relationshipSearch.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, landingSearch.StatusCode);
        Assert.Equal(HttpStatusCode.OK, relationshipSearch.StatusCode);
        Assert.Contains("Thabo Mokoena", landingJson);
        Assert.DoesNotContain("Thabo Mokoena", relationshipJson);
        Assert.Contains("Delivery", relationshipJson);
    }

    [Fact]
    public async Task Profiles_route_redirects_to_relationships()
    {
        using var response = await _client.GetAsync("/Profiles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("/Relationships", response.RequestMessage?.RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task Me_verify_and_admin_pages_expose_polished_workspaces()
    {
        using var me = await _client.GetAsync("/Me");
        using var verify = await _client.GetAsync("/Verify");
        using var admin = await _client.GetAsync("/Admin");
        var meHtml = await me.Content.ReadAsStringAsync();
        var verifyHtml = await verify.Content.ReadAsStringAsync();
        var adminHtml = await admin.Content.ReadAsStringAsync();

        Assert.Contains("My profile", meHtml);
        Assert.Contains("Owned vehicle records", meHtml);
        Assert.Contains("Current relationships", meHtml);
        Assert.Contains("Profile type", verifyHtml);
        Assert.Contains("Contested queue", adminHtml);
        Assert.Contains("Seed coverage", adminHtml);
    }

    [Fact]
    public async Task Verify_rules_and_admin_dashboard_endpoints_are_available()
    {
        using var rules = await _client.GetAsync("/Verify/Rules?profileType=Owner");
        using var adminDashboard = await _client.GetAsync("/Admin/Dashboard?market=Fleet");
        var rulesJson = await rules.Content.ReadAsStringAsync();
        var adminJson = await adminDashboard.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, rules.StatusCode);
        Assert.Equal(HttpStatusCode.OK, adminDashboard.StatusCode);
        Assert.Contains("allowedCaseTypes", rulesJson);
        Assert.Contains("adminTrustSignals", adminJson);
    }
}
