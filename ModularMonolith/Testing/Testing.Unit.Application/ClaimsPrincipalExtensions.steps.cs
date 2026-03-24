using System.Security.Claims;
using Application.Authentication;
using Shouldly;

namespace Application;

public partial class ClaimsPrincipalExtensionsSpecs
{
    private ClaimsPrincipal user = null!;
    private Guid userId;

    private static readonly Guid KnownUserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    private void a_claims_principal_with_valid_sub_claim() =>
        user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", KnownUserId.ToString())]));

    private void a_claims_principal_with_no_sub_claim() =>
        user = new ClaimsPrincipal(new ClaimsIdentity([]));

    private void a_claims_principal_with_invalid_sub_claim() =>
        user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "not-a-guid")]));

    private void getting_user_id() => userId = user.GetUserId();

    private void the_user_id_is_returned() => userId.ShouldBe(KnownUserId);
}