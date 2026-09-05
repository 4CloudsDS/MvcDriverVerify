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
        using var relationshipSearch = await _client.GetAsync("/Relationships/Search?query=Fleet&mode=opportunity&relationshipType=Fleet%20contract");

        Assert.Equal(HttpStatusCode.OK, landingSearch.StatusCode);
        Assert.Equal(HttpStatusCode.OK, relationshipSearch.StatusCode);
    }

    [Fact]
    public async Task Profiles_route_redirects_to_relationships()
    {
        using var response = await _client.GetAsync("/Profiles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("/Relationships", response.RequestMessage?.RequestUri?.AbsolutePath);
    }
}
