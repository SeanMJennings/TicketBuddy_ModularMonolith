using BDD;
using Domain.Users.Entities;
using Domain.Users.Primitives;
using Shouldly;

namespace Unit;

public partial class UserSpecs : Specification
{
    private Guid id;
    private string name = null!;
    private string email = null!;
    private User user = null!;

    private const string invalid_email = "wibble";
    private const string invalid_name = "Jackie Chan 123";
    private const string valid_email = "wibble@wobble.com";
    private const string valid_name = "Jackie Chan";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        name = null!;
        email = null!;
        user = null!;
    }

    private void valid_inputs()
    {
        name = valid_name;
        email = valid_email;
    }

    private void a_null_user_name()
    {
        name = null!;
    }    
    
    private void an_empty_user_name()
    {
        name = string.Empty;
    }     
    
    private void a_user_name_with_non_alphabetical_characters()
    {
        name = invalid_name;
    }    
    
    private void a_null_email()
    {
        email = null!;
    }    
    
    private void an_empty_email()
    {
        email = string.Empty;
    }    
    
    private void an_invalid_email()
    {
        email = invalid_email;
    }

    private void creating_a_user()
    {
        user = new User(id, name, email, UserType.Customer);
    }    
    
    private void the_user_is_created()
    {
        user.Id.ShouldBe(id);
        user.FullName.ToString().ShouldBe(valid_name);
        user.Email.ToString().ShouldBe(valid_email);
        user.UserType.ShouldBe(UserType.Customer);
    }
}