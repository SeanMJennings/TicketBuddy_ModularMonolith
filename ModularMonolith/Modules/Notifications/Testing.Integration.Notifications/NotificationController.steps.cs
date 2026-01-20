using System.Security.Claims;
using Controllers.Notifications;
using Domain.Notifications;
using Infrastructure.Configuration;
using Infrastructure.Notifications.Core;
using Infrastructure.Notifications.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

namespace Integration;

public partial class NotificationControllerSpecs : TruncateDbSpecification
{
    private GetNotificationsEndpoint getNotificationsEndpoint = null!;
    private MarkNotificationAsReadEndpoint markNotificationAsReadEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private static PostgreSqlContainer database = null!;
    private Guid userId = Guid.NewGuid();
    private Guid notificationId = Guid.NewGuid();
    private List<NotificationResponse> returnedNotifications = [];

    private readonly DateTimeOffset olderCreatedAt = DateTimeOffset.UtcNow.AddHours(-2);
    private readonly DateTimeOffset newerCreatedAt = DateTimeOffset.UtcNow.AddHours(-1);

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.NewGuid();
        notificationId = Guid.NewGuid();
        returnedNotifications = [];

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<GetNotificationsEndpoint>()
            .AddScoped<MarkNotificationAsReadEndpoint>()
            .BuildServiceProvider();

        getNotificationsEndpoint = serviceProvider.GetRequiredService<GetNotificationsEndpoint>();
        markNotificationAsReadEndpoint = serviceProvider.GetRequiredService<MarkNotificationAsReadEndpoint>();
        AddUserClaimToControllerContext(userId);
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

    private void AddUserClaimToControllerContext(Guid theUserId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", theUserId.ToString())], "TestAuth"));
        getNotificationsEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        markNotificationAsReadEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private async Task PersistNotification(Notification notification)
    {
        var repository = serviceProvider.GetRequiredService<IPersistNotifications>();
        var dbContext = serviceProvider.GetRequiredService<NotificationDbContext>();
        await repository.Add(notification);
        await dbContext.Commit();
    }

    private async Task notifications_exist_for_user()
    {
        var olderNotification = Notification.Create(
            Guid.NewGuid(),
            userId,
            "TicketPurchased",
            "{\"eventName\":\"Concert A\"}",
            olderCreatedAt);

        var newerNotification = Notification.Create(
            Guid.NewGuid(),
            userId,
            "TicketPurchased",
            "{\"eventName\":\"Concert B\"}",
            newerCreatedAt);

        await PersistNotification(olderNotification);
        await PersistNotification(newerNotification);
    }

    private async Task requesting_notifications()
    {
        returnedNotifications = (await getNotificationsEndpoint.GetNotifications()).ToList();
    }

    private void notifications_are_returned_ordered_by_created_at_descending()
    {
        returnedNotifications.Count.ShouldBe(2);
        returnedNotifications[0].CreatedAt.ShouldBe(newerCreatedAt);
        returnedNotifications[1].CreatedAt.ShouldBe(olderCreatedAt);
    }

    private async Task an_unread_notification_exists()
    {
        var notification = Notification.Create(
            notificationId,
            userId,
            "TicketPurchased",
            "{\"eventName\":\"Concert\"}",
            DateTimeOffset.UtcNow);

        await PersistNotification(notification);
    }

    private async Task marking_notification_as_read()
    {
        await markNotificationAsReadEndpoint.MarkAsRead(notificationId);
    }

    private async Task the_notification_is_marked_as_read()
    {
        var repository = serviceProvider.GetRequiredService<IPersistNotifications>();
        var notification = await repository.GetById(notificationId);
        notification.ShouldNotBeNull();
        notification.IsRead.ShouldBeTrue();
    }
}