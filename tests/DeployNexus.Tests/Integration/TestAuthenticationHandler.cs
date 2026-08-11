using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeployNexus.Tests.Integration;

public class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(
                "X-Test-UserId",
                out var userId))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Invalid test user ID."));
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                parsedUserId.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            SchemeName);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SchemeName);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}