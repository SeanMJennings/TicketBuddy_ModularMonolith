using NUnit.Framework;

namespace Integration;

public partial class UserControllerSpecs
{
    [Test]
    public async Task can_create_user()
    {
              Given(a_request_to_create_an_user);
        await When(creating_the_user);
        await And(requesting_the_user);
              Then(the_user_is_created);
    }
    
    [Test]
    public async Task can_update_user()
    {
        await Given(a_user_exists);
              And(a_request_to_update_the_user);
        await When(updating_the_user);
        await And(requesting_the_updated_user);
              Then(the_user_is_updated);
    }
    
    [Test]
    public async Task can_list_users()
    {
        await Given(a_user_exists);
        await And(another_user_exists);
        await When(listing_the_users);
              Then(the_users_are_listed);
    }
    
    [Test]
    public async Task cannot_create_user_with_duplicate_email()
    {
        await Given(a_user_exists);
              And(a_request_to_create_a_user_with_same_email);
        await When(creating_the_user_which_fails);
              Then(email_already_exists);
    }
    
    [Test]
    public async Task cannot_update_user_with_duplicate_email()
    {
        await Given(a_user_exists);
        await And(another_user_exists);
              And(a_request_to_update_user_with_duplicate_email);
        await When(updating_another_user_which_fails);
              Then(email_already_exists);
    }
}