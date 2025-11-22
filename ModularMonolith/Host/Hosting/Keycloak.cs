using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Hosting;

internal static class Keycloak
{
    internal static IServiceCollection ConfigureKeycloakAuth(this IServiceCollection services, Settings.KeycloakSettings settings)
    {
        services
            .AddAuthentication()
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.MetadataAddress = $"{settings.ServerUrl}/realms/ticketbuddy/.well-known/openid-configuration";
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = ClaimTypes.Role,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = $"{settings.ServerUrl}/realms/ticketbuddy",
                };
                
                RetrieveRealmAccessRolesFromJwt(x);
            });
        
        return services;
    }

    private static void RetrieveRealmAccessRolesFromJwt(JwtBearerOptions x)
    {
        x.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                if (ctx.Principal?.Identity is not CaseSensitiveClaimsIdentity identity) return Task.CompletedTask;

                var realmAccess = ctx.Principal.FindFirst("realm_access")?.Value;
                if (string.IsNullOrEmpty(realmAccess)) return Task.CompletedTask;
                        
                try
                {
                    using var doc = JsonDocument.Parse(realmAccess);
                    if (doc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        foreach (var r in roles.EnumerateArray())
                        {
                            var role = r.GetString();
                            if (!string.IsNullOrEmpty(role)) identity.AddClaim(new Claim(identity.RoleClaimType, role));
                        }
                    }
                }
                catch (JsonException) { }
                catch (ArgumentException) { }

                return Task.CompletedTask;
            }
        };
    }
}