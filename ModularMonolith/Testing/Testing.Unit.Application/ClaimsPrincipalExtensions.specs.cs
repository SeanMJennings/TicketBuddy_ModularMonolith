using BDD;
using NUnit.Framework;

namespace Application;

public partial class ClaimsPrincipalExtensionsSpecs : Specification
{
    [Test]
    public void can_get_user_id_from_valid_sub_claim()
    {
        Given(a_claims_principal_with_valid_sub_claim);
        When(getting_user_id);
        Then(the_user_id_is_returned);
    }

    [Test]
    public void throws_when_sub_claim_is_missing()
    {
        Given(a_claims_principal_with_no_sub_claim);
        When(Validating(getting_user_id));
        Then(Informs("User id claim invalid"));
    }

    [Test]
    public void throws_when_sub_claim_is_not_a_valid_guid()
    {
        Given(a_claims_principal_with_invalid_sub_claim);
        When(Validating(getting_user_id));
        Then(Informs("User id claim invalid"));
    }
}