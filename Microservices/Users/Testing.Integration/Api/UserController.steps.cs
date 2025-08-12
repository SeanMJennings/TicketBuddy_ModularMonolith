﻿using System.Net;
using System.Text;
using Api.Hosting;
using Api.Requests;
using BDD;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shared.Testing;
using Shouldly;
using Testcontainers.MsSql;
using Users.Domain.Entities;
using Users.Integration.Messaging.Outbound.Messages;
using WebHost;

namespace Integration.Api;

public partial class UserControllerSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid returned_id;
    private Guid another_id;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string email = "wibble@wobble.com";
    private const string new_name = "wobble";
    private const string new_email = "wobble@wibble.com";
    private static MsSqlContainer database = null!;
    private ITestHarness testHarness = null!;

    protected override void before_all()
    {
        database = new MsSqlBuilder().WithPortBinding(1433, true).Build();
        database.StartAsync().Await();
        database.ExecScriptAsync("CREATE DATABASE [TicketBuddy.Users]").GetAwaiter().GetResult();
        Migration.Upgrade(database.GetTicketBuddyConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetTicketBuddyConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
    }

    protected override void after_each()
    {
        Truncate(database.GetTicketBuddyConnectionString());
        testHarness.Stop().Await();
        client.Dispose();
        factory.Dispose();
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
        content = new StringContent(
            JsonSerialization.Serialize(new UserPayload(the_name, the_email)), 
            Encoding.UTF8, 
            application_json);
    }

    private void a_request_to_create_another_user()
    {
        create_content(new_name, new_email);

    }
    
    private void a_request_to_update_the_user()
    {
        create_content(new_name, new_email);
    }

    private void a_request_to_create_a_user_with_same_email()
    {
        create_content(name, email);
    }
    
    private void a_request_to_update_user_with_duplicate_email()
    {
        create_content(name, email);
    }

    private void creating_the_user()
    {
        var response = client.PostAsync(UserRoutes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_another_user()
    {
        var response = client.PostAsync(UserRoutes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_the_user_which_fails()
    {
        var response = client.PostAsync(UserRoutes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        content = response.Content;
    }
    
    private void updating_the_user()
    {
        var response = client.PutAsync(UserRoutes.Users + $"/{returned_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }
    
    private void updating_another_user_which_fails()
    {
        var response = client.PutAsync(UserRoutes.Users + $"/{another_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        content = response.Content;
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
        var response = client.GetAsync(UserRoutes.Users + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void requesting_the_updated_user()
    {
        var response = client.GetAsync(UserRoutes.Users + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void listing_the_users()
    {
        var response = client.GetAsync(UserRoutes.Users).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_user_is_created()
    {
        var theuser = JsonSerialization.Deserialize<User>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theuser.Id.ShouldBe(returned_id);
        theuser.FullName.ToString().ShouldBe(name);
        theuser.Email.ToString().ShouldBe(email);
        testHarness.Published.Select<Users.Domain.Messaging.Messages.UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: name, Email: email }).ShouldBeTrue("user was not published to the bus");
        testHarness.Published.Select<UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: name, Email: email }).ShouldBeTrue("user was not published to the bus");
    }
    
    private void the_user_is_updated()
    {
        var theUser = JsonSerialization.Deserialize<User>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(new_name);
        theUser.Email.ToString().ShouldBe(new_email);
        testHarness.Published.Select<Users.Domain.Messaging.Messages.UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: name, Email: email }).ShouldBeTrue("user was not published to the bus");
        testHarness.Published.Select<UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: new_name, Email: new_email }).ShouldBeTrue("user was not published to the bus");
        testHarness.Published.Select<Users.Domain.Messaging.Messages.UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: name, Email: email }).ShouldBeTrue("user was not published to the bus");
        testHarness.Published.Select<UserUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message is { FullName: new_name, Email: new_email }).ShouldBeTrue("user was not published to the bus");
    }    
    
    private void the_users_are_listed()
    {
        var theUser = JsonSerialization.Deserialize<IReadOnlyList<User>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Count.ShouldBe(2);
        theUser.Single(e => e.Id == returned_id).FullName.ToString().ShouldBe(name);
        theUser.Single(e => e.Id == another_id).FullName.ToString().ShouldBe(new_name);
    }

    private void email_already_exists()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().GetAwaiter().GetResult()).Errors.ShouldContain("Email already exists");
    }
}