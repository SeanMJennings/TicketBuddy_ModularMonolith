using BDD;
using Domain.Tickets.User;
using Shouldly;

namespace Unit;

public partial class UserSpecs : Specification
{
    private Guid id;
    private string fullName = null!;
    private string email = null!;
    private User theUser = null!;

    private const string valid_full_name = "John Smith";
    private const string invalid_full_name = "John Smith 123!";
    private const string updated_full_name = "Jane Doe";
    private const string valid_email = "john@example.com";
    private const string invalid_email = "not-an-email";
    private const string updated_email = "jane@example.com";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.CreateVersion7();
        fullName = null!;
        email = null!;
        theUser = null!;
    }

    private void valid_user_inputs()
    {
        fullName = valid_full_name;
        email = valid_email;
    }

    private void a_null_name() => fullName = null!;
    private void an_empty_name() => fullName = string.Empty;
    private void a_name_with_invalid_characters() => fullName = invalid_full_name;
    private void a_null_email() => email = null!;
    private void an_empty_email() => email = string.Empty;
    private void an_invalid_email() => email = invalid_email;

    private void a_valid_user()
    {
        valid_user_inputs();
        creating_a_user();
    }

    private void creating_a_user()
    {
        theUser = new User(id, new Name(fullName), new Email(email));
    }

    private void updating_user_name()
    {
        theUser.UpdateName(new Name(updated_full_name));
    }

    private void updating_user_email()
    {
        theUser.UpdateEmail(new Email(updated_email));
    }

    private void the_user_is_created()
    {
        theUser.ShouldNotBeNull();
        theUser.Id.ShouldBe(id);
        (theUser.FullName == valid_full_name).ShouldBeTrue();
        (theUser.FullName != valid_full_name).ShouldBeFalse();
        theUser.FullName.GetHashCode().ShouldBe(new Name(valid_full_name).GetHashCode());
        (theUser.Email == valid_email).ShouldBeTrue();
        (theUser.Email != valid_email).ShouldBeFalse();
        theUser.Email.GetHashCode().ShouldBe(new Email(valid_email).GetHashCode());
    }

    private void user_name_is_updated()
    {
        string updatedName = theUser.FullName;
        updatedName.ShouldBe(updated_full_name);
        (theUser.FullName != valid_full_name).ShouldBeTrue();
        theUser.FullName.GetHashCode().ShouldBe(new Name(updated_full_name).GetHashCode());
        theUser.FullName.GetHashCode().ShouldNotBe(new Name(valid_full_name).GetHashCode());
    }

    private void user_email_is_updated()
    {
        string updatedEmailStr = theUser.Email;
        updatedEmailStr.ShouldBe(updated_email);
        (theUser.Email != valid_email).ShouldBeTrue();
        theUser.Email.GetHashCode().ShouldBe(new Email(updated_email).GetHashCode());
        theUser.Email.GetHashCode().ShouldNotBe(new Email(valid_email).GetHashCode());
    }

    private bool nameEqualityResult;
    private bool emailEqualityResult;

    private void comparing_name_to_non_name_object()
    {
        object nonName = "not a name";
        nameEqualityResult = new Name(valid_full_name).Equals(nonName);
    }

    private void comparing_email_to_non_email_object()
    {
        object nonEmail = "not an email";
        emailEqualityResult = new Email(valid_email).Equals(nonEmail);
    }

    private void name_is_not_equal() => nameEqualityResult.ShouldBeFalse();
    private void email_is_not_equal() => emailEqualityResult.ShouldBeFalse();
}