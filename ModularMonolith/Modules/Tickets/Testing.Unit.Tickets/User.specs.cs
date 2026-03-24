using NUnit.Framework;

namespace Unit;

public partial class UserSpecs
{
    [Test]
    public void can_create_a_user()
    {
        Given(valid_user_inputs);
        When(creating_a_user);
        Then(the_user_is_created);
    }

    [Test]
    public void a_users_name_must_be_alphabetical()
    {
        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(a_null_name);
            When(Validating(creating_a_user));
            Then(Informs("Name cannot be null or empty"));
        });

        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(an_empty_name);
            When(Validating(creating_a_user));
            Then(Informs("Name cannot be null or empty"));
        });

        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(a_name_with_invalid_characters);
            When(Validating(creating_a_user));
            Then(Informs("Name can only have alphabetical characters"));
        });
    }

    [Test]
    public void a_users_email_must_be_valid()
    {
        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(a_null_email);
            When(Validating(creating_a_user));
            Then(Informs("Email cannot be null or empty"));
        });

        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(an_empty_email);
            When(Validating(creating_a_user));
            Then(Informs("Email cannot be null or empty"));
        });

        Scenario(() =>
        {
            Given(valid_user_inputs);
            And(an_invalid_email);
            When(Validating(creating_a_user));
            Then(Informs("Email must be valid"));
        });
    }

    [Test]
    public void can_update_users_name()
    {
        Given(a_valid_user);
        When(updating_user_name);
        Then(user_name_is_updated);
    }

    [Test]
    public void can_update_users_email()
    {
        Given(a_valid_user);
        When(updating_user_email);
        Then(user_email_is_updated);
    }

    [Test]
    public void a_name_is_not_equal_to_a_non_name_object()
    {
        Given(valid_user_inputs);
        When(comparing_name_to_non_name_object);
        Then(name_is_not_equal);
    }

    [Test]
    public void an_email_is_not_equal_to_a_non_email_object()
    {
        Given(valid_user_inputs);
        When(comparing_email_to_non_email_object);
        Then(email_is_not_equal);
    }
}