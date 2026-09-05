using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MvcDriverVerify.Security;

public sealed class ConfiguredRoleAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ConfiguredRole";

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public ConfiguredRoleAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration,
        IHostEnvironment environment)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
        _environment = environment;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var roles = _configuration.GetSection("UserExperience:Roles").Get<string[]>()
            ?? (_environment.IsDevelopment()
                ? ["PublicUser", "Driver", "Owner", "Fleet", "Platform", "Moderator", "Admin"]
                : ["PublicUser"]);
        var displayName = _configuration["UserExperience:DisplayName"] ?? "Demo user";

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, displayName)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
