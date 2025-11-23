using System.Security.Claims;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Component;

public class UserHeaders
{
    public const string UserType = "X-Test-User-Type";
    public const string UserId = "X-Test-User-Id";
}

public class FakeAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    System.Text.Encodings.Web.UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserHeaders.UserType, out var userTypeValues))
            return Task.FromResult(AuthenticateResult.Fail("Missing test user header"));

        var userType = userTypeValues.ToString();
        if (string.IsNullOrWhiteSpace(userType))
            return Task.FromResult(AuthenticateResult.Fail("Empty test user header"));

        if (!Enum.TryParse<UserType>(userType, ignoreCase: true, out _))
            return Task.FromResult(AuthenticateResult.Fail("Invalid test user type"));

        var roleClaim = userType switch
        {
            nameof(UserType.Admin) => Roles.Admin,
            nameof(UserType.Customer) => Roles.Customer,
            _ => null
        };

        Request.Headers.TryGetValue(UserHeaders.UserId, out var userIdValues);
        
        var userId = userIdValues.ToString();
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, roleClaim!),
            new Claim("sub", userId)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
