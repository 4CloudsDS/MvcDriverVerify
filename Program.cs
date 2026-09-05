using Microsoft.AspNetCore.Authentication;
using MvcDriverVerify.Security;
using MvcDriverVerify.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<DriverMarketplaceService>(client =>
{
    var baseUrl = builder.Configuration["DriverVerificationApi:BaseUrl"] ?? "http://localhost:5031";
    client.BaseAddress = new Uri(baseUrl);

    var apiKey = builder.Configuration["DriverVerificationApi:ApiKey"];
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }
});
builder.Services.AddAuthentication(ConfiguredRoleAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ConfiguredRoleAuthenticationHandler>(
        ConfiguredRoleAuthenticationHandler.SchemeName,
        options => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DriverWorkspace", policy => policy.RequireRole("Driver", "Owner", "Fleet", "Platform", "Moderator", "Admin"));
    options.AddPolicy("OwnerWorkspace", policy => policy.RequireRole("Owner", "Fleet", "Platform", "Moderator", "Admin"));
    options.AddPolicy("AdminWorkspace", policy => policy.RequireRole("Moderator", "Admin"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program
{
}
