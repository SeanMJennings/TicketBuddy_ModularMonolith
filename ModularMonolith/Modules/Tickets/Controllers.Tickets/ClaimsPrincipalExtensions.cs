using System.Security.Claims;

namespace Controllers.Tickets;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var id = user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException("User id claim not present.");
        return !Guid.TryParse(id, out var guid) ? throw new InvalidOperationException("User id claim is not a valid GUID.") : guid;
    }
}