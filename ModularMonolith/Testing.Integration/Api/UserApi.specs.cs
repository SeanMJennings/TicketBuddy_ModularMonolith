using NUnit.Framework;

namespace Integration.Api;

public partial class UserApiSpecs
{
    [Test]
    public void can_create_user()
    {
        Given(a_request_to_create_an_user);
        When(creating_the_user);
        And(requesting_the_user);
        Then(the_user_is_created);
    }
    
    [Test]
    public void can_update_user()
    {
        Given(a_user_exists);
        And(a_request_to_update_the_user);
        When(updating_the_user);
        And(requesting_the_updated_user);
        Then(the_user_is_updated);
    }
    
    [Test]
    public void can_list_users()
    {
        Given(a_user_exists);
        And(another_user_exists);
        When(listing_the_users);
        Then(the_users_are_listed);
    }
    
    [Test]
    public void cannot_create_user_with_duplicate_email()
    {
        Given(a_user_exists);
        And(a_request_to_create_a_user_with_same_email);
        When(creating_the_user_which_fails);
        Then(email_already_exists);
    }
    
    [Test]
    public void cannot_update_user_with_duplicate_email()
    {
        Given(a_user_exists);
        And(another_user_exists);
        And(a_request_to_update_user_with_duplicate_email);
        When(updating_another_user_which_fails);
        Then(email_already_exists);
    }
}