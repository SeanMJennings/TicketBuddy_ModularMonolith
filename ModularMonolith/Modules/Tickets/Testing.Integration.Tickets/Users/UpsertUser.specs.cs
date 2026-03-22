using NUnit.Framework;

namespace Integration.Tickets;

public partial class UpsertUserSpecs
{
    [Test]
    public async Task upserting_a_user_that_already_exists_updates_their_details()
    {
        await Given(the_user_has_been_registered);
        await When(the_user_registers_again_with_new_details);
        await Then(the_user_record_is_updated);
    }
}