using System.ComponentModel.DataAnnotations;
using Controllers.Users;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Domain.Users.Primitives;
using Infrastructure.Configuration;
using Infrastructure.Users.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

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

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }
    
    protected override Task before_each()
    {
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
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
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

    private async Task creating_the_user()
    {
        returned_id = Guid.Parse((await userController.CreateUser(userPayload)).Value!.ToString()!);
    }
    
    private async Task creating_another_user()
    {
        another_id = Guid.Parse((await userController.CreateUser(userPayload)).Value!.ToString()!);
    }
    
    private async Task creating_the_user_which_fails()
    {
        try
        {
            await userController.CreateUser(userPayload);
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private async Task updating_the_user()
    {
        await userController.UpdateUser(returned_id, updateUserPayload);
    }
    
    private async Task updating_another_user_which_fails()
    {
        try
        {
            await userController.UpdateUser(another_id, updateUserPayload);
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private async Task a_user_exists()
    {
        a_request_to_create_an_user();
        await creating_the_user();
    }  
    
    private async Task another_user_exists()
    {
        a_request_to_create_another_user();
        await creating_another_user();
    }

    private async Task requesting_the_user()
    {
        theUser = (await userController.GetUser(returned_id)).Value!;
    }
    
    private async Task requesting_the_updated_user()
    {
        theUser = (await userController.GetUser(returned_id)).Value!;
    }
    
    private async Task listing_the_users()
    {
        theUsers = (await userController.GetUsers()).ToList();
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