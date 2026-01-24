using System.Security.Claims;

namespace Application.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return !Guid.TryParse(user.FindFirst("sub")?.Value, out var guid) 
            ? throw new InvalidOperationException("User id claim invalid") : guid;
    }
}