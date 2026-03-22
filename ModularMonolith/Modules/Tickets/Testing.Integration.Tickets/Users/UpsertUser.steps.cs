using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Tickets.Core;
using Infrastructure.Tickets.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Messaging.Keycloak.Users;
using Messaging.Tickets.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testing;

namespace Integration.Tickets;

public partial class UpsertUserSpecs : TruncateDbSpecification
{
    private UserRegisteredConsumer userRegisteredConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private ITestHarness testHarness = null!;

    private Guid user_id = Guid.CreateVersion7();
    private const string original_email = "john.smith@example.com";
    private const string updated_email = "john.updated@example.com";

    protected override Task before_each()
    {
        user_id = Guid.CreateVersion7();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureCache(Setup.Redis.GetConnectionString())
            .ConfigureTicketsDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddTicketsConsumers();
                x.AddTicketsOutbox();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .ConfigureTicketsServices()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().GetAwaiter().GetResult();
        userRegisteredConsumer = serviceProvider.GetRequiredService<UserRegisteredConsumer>();
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
        await testHarness.Stop();
    }

    private async Task the_user_has_been_registered()
    {
        var context = Substitute.For<ConsumeContext<UserRegistered>>();
        context.Message.Returns(new UserRegistered(user_id, new Dictionary<string, string>
        {
            { "first_name", "John" },
            { "last_name", "Smith" },
            { "email", original_email }
        }));
        await userRegisteredConsumer.Consume(context);
    }

    private async Task the_user_registers_again_with_new_details()
    {
        var context = Substitute.For<ConsumeContext<UserRegistered>>();
        context.Message.Returns(new UserRegistered(user_id, new Dictionary<string, string>
        {
            { "first_name", "Johnny" },
            { "last_name", "Updated" },
            { "email", updated_email }
        }));
        await userRegisteredConsumer.Consume(context);
    }

    private async Task the_user_record_is_updated()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
        var users = await dbContext.Users.ToListAsync();
        users.Count.ShouldBe(1);
        var user = users.Single();
        user.Id.ShouldBe(user_id);
        (user.FullName == "Johnny Updated").ShouldBeTrue();
        (user.Email == updated_email).ShouldBeTrue();
    }
}