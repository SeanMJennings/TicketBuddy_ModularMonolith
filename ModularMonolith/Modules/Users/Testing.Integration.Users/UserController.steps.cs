using System.ComponentModel.DataAnnotations;
using BDD;
using Controllers.Users;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Domain.Users.Primitives;
using Infrastructure.Configuration;
using Infrastructure.Users.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;

namespace Integration;

public partial class UserControllerSpecs : TruncateDbSpecification
{
    private UserController userController = null!;
    private ServiceProvider serviceProvider = null!;
    private UserPayload userPayload = null!;
    private UpdateUserPayload updateUserPayload = null!;
    private ValidationException theError = null!;
    private User theUser = null!;
    private List<User> theUsers = [];
    
    private Guid returned_id;
    private Guid another_id;
    private const string name = "wibble";
    private const string email = "wibble@wobble.com";
    private const string new_name = "wobble";
    private const string new_email = "wobble@wibble.com";
    private static PostgreSqlContainer database = null!;

    protected override void before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1434, true)
            .Build();
        database.StartAsync().Await();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        returned_id = Guid.Empty;
        another_id = Guid.Empty;
        userPayload = null!;
        updateUserPayload = null!;
        theError = null!;
        theUser = null!;
        theUsers = [];
        
        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureUsersServices()
            .ConfigureUsersDatabase(database.GetConnectionString())
            .AddMassTransitTestHarness()
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<UserController>()
            .BuildServiceProvider();
        
        userController = serviceProvider.GetRequiredService<UserController>();
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
    }

    protected override void after_all()
    {
        database.StopAsync().Await();
        database.DisposeAsync().GetAwaiter().GetResult();
    }

    private void a_request_to_create_an_user()
    {
        create_content(name, email);
    }

    private void create_content(string the_name, string the_email)
    {
        userPayload = new UserPayload(the_name, the_email, UserType.Administrator);
    }    
    
    private void create_update_content(string the_name, string the_email)
    {
        updateUserPayload = new UpdateUserPayload(the_name, the_email);
    }

    private void a_request_to_create_another_user()
    {
        create_content(new_name, new_email);

    }
    
    private void a_request_to_update_the_user()
    {
        create_update_content(new_name, new_email);
    }

    private void a_request_to_create_a_user_with_same_email()
    {
        create_content(name, email);
    }
    
    private void a_request_to_update_user_with_duplicate_email()
    {
        create_update_content(name, email);
    }

    private void creating_the_user()
    {
        returned_id = Guid.Parse(userController.CreateUser(userPayload).Await().Value!.ToString());
    }
    
    private void creating_another_user()
    {
        another_id = Guid.Parse(userController.CreateUser(userPayload).Await().Value!.ToString());
    }
    
    private void creating_the_user_which_fails()
    {
        try
        {
            userController.CreateUser(userPayload).Await();
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private void updating_the_user()
    {
        userController.UpdateUser(returned_id, updateUserPayload).Await();
    }
    
    private void updating_another_user_which_fails()
    {
        try
        {
            userController.UpdateUser(another_id, updateUserPayload).Await();
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private void a_user_exists()
    {
        a_request_to_create_an_user();
        creating_the_user();
    }  
    
    private void another_user_exists()
    {
        a_request_to_create_another_user();
        creating_another_user();
    }

    private void requesting_the_user()
    {
        theUser = userController.GetUser(returned_id).Await().Value!;
    }
    
    private void requesting_the_updated_user()
    {
        theUser = userController.GetUser(returned_id).Await().Value!;
    }
    
    private void listing_the_users()
    {
        theUsers = userController.GetUsers().Await().ToList();
    }

    private void the_user_is_created()
    {
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(name);
        theUser.Email.ToString().ShouldBe(email);
        theUser.UserType.ShouldBe(UserType.Administrator);
    }
    
    private void the_user_is_updated()
    {
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(new_name);
        theUser.Email.ToString().ShouldBe(new_email);
    }    
    
    private void the_users_are_listed()
    {
        theUsers.Count.ShouldBe(2);
        theUsers.Single(e => e.Id == returned_id).FullName.ToString().ShouldBe(name);
        theUsers.Single(e => e.Id == another_id).FullName.ToString().ShouldBe(new_name);
    }

    private void email_already_exists()
    {
        theError.Message.ShouldContain("Email already exists");
    }
}