using System.Net;
using System.Text;
using Controllers.Users;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Domain.Users.Primitives;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing.Containers;

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

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }
    
    protected override Task before_each()
    {
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await  Truncate(database.GetConnectionString());
        client.Dispose();
        await factory.DisposeAsync();
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

    private async Task creating_the_user()
    {
        var response = await client.PostAsync(Routes.Users, content);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(await response.Content.ReadAsStringAsync());
    }
    
    private async Task creating_another_user()
    {
        var response = await client.PostAsync(Routes.Users, content);
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(await response.Content.ReadAsStringAsync());
    }
    
    private async Task updating_the_user()
    {
        var response = await client.PutAsync(Routes.Users + $"/{returned_id}", content);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
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
        var response = await client.GetAsync(Routes.Users + $"/{returned_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private async Task requesting_the_updated_user()
    {
        var response = await client.GetAsync(Routes.Users + $"/{returned_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private async Task listing_the_users()
    {
        var response =await client.GetAsync(Routes.Users);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task the_user_is_created()
    {
        var theUser = JsonSerialization.Deserialize<User>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(name);
        theUser.Email.ToString().ShouldBe(email);
        theUser.UserType.ShouldBe(UserType.Administrator);
    }
    
    private async Task the_user_is_updated()
    {
        var theUser = JsonSerialization.Deserialize<User>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Id.ShouldBe(returned_id);
        theUser.FullName.ToString().ShouldBe(new_name);
        theUser.Email.ToString().ShouldBe(new_email);
    }    
    
    private async Task the_users_are_listed()
    {
        var theUser = JsonSerialization.Deserialize<IReadOnlyList<User>>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theUser.Count.ShouldBe(2);
        theUser.Single(e => e.Id == returned_id).FullName.ToString().ShouldBe(name);
        theUser.Single(e => e.Id == another_id).FullName.ToString().ShouldBe(new_name);
    }
}