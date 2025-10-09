using System.Net;
using System.Text;
using BDD;
using Controllers.Users;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Domain.Users.Primitives;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using WebHost;

namespace Component.Api;

public partial class UserApiSpecs : TruncateDbSpecification
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
    private static PostgreSqlContainer database = null!;

    protected override void before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1433, true)
            .Build();
        database.StartAsync().Await();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
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
            JsonSerialization.Serialize(new UserPayload(the_name, the_email, UserType.Administrator)), 
            Encoding.UTF8, 
            application_json);
    }    
    
    private void create_update_content(string the_name, string the_email)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new UpdateUserPayload(the_name, the_email)), 
            Encoding.UTF8, 
            application_json);
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
        var response = client.PostAsync(Routes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_another_user()
    {
        var response = client.PostAsync(Routes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_the_user_which_fails()
    {
        var response = client.PostAsync(Routes.Users, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        content = response.Content;
    }
    
    private void updating_the_user()
    {
        var response = client.PutAsync(Routes.Users + $"/{returned_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }
    
    private void updating_another_user_which_fails()
    {
        var response = client.PutAsync(Routes.Users + $"/{another_id}", content).GetAwaiter().GetResult();
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
        var response = client.GetAsync(Routes.Users + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void requesting_the_updated_user()
    {
        var response = client.GetAsync(Routes.Users + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void listing_the_users()
    {
        var response = client.GetAsync(Routes.Users).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_user_is_created()
    {
        var theUser = JsonSerialization.Deserialize<User>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(name);
        theUser.Email.ToString().ShouldBe(email);
        theUser.UserType.ShouldBe(UserType.Administrator);
    }
    
    private void the_user_is_updated()
    {
        var theUser = JsonSerialization.Deserialize<User>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(new_name);
        theUser.Email.ToString().ShouldBe(new_email);
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