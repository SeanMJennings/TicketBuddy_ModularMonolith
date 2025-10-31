using NUnit.Framework;

namespace Component.Api;

public partial class UserApiSpecs
{
    [Test]
    public async Task can_create_user()
    {
              Given(a_request_to_create_an_user);
        await When(creating_the_user);
        await And(requesting_the_user);
        await Then(the_user_is_created);
    }
    
    [Test]
    public async Task can_update_user()
    {
        await Given(a_user_exists);
              And(a_request_to_update_the_user);
        await When(updating_the_user);
        await And(requesting_the_updated_user);
        await Then(the_user_is_updated);
    }
    
    [Test]
    public async Task can_list_users()
    {
        await Given(a_user_exists);
        await And(another_user_exists);
        await When(listing_the_users);
        await Then(the_users_are_listed);
    }
}