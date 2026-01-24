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
using Testing;

namespace Integration.Notifications;

public partial class GetNotificationsSpecs : TruncateDbSpecification
{
    private GetNotificationsEndpoint getNotificationsEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private Guid userId = Guid.NewGuid();
    private List<NotificationResponse> returnedNotifications = [];
    private readonly DateTimeOffset olderCreatedAt = DateTimeOffset.UtcNow.AddHours(-2);
    private readonly DateTimeOffset newerCreatedAt = DateTimeOffset.UtcNow.AddHours(-1);

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.NewGuid();
        returnedNotifications = [];

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(Setup.Database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<GetNotificationsEndpoint>()
            .BuildServiceProvider();

        getNotificationsEndpoint = serviceProvider.GetRequiredService<GetNotificationsEndpoint>();
        AddUserClaimToControllerContext(userId);
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
    }

    private void AddUserClaimToControllerContext(Guid theUserId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", theUserId.ToString())], "TestAuth"));
        getNotificationsEndpoint.ControllerContext = new ControllerContext
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

    private Notification CreateNotification(
        Guid? id = null,
        Guid? forUserId = null,
        string eventName = "Concert",
        DateTimeOffset? createdAt = null,
        bool isRead = false)
    {
        var notification = Notification.Create(
            id ?? Guid.NewGuid(),
            forUserId ?? userId,
            NotificationType.TicketPurchased,
            $"{{\"eventName\":\"{eventName}\"}}",
            createdAt ?? DateTimeOffset.UtcNow);

        if (isRead)
            notification.MarkAsRead();

        return notification;
    }

    private async Task notifications_exist_for_user()
    {
        await PersistNotification(CreateNotification(eventName: "Concert A", createdAt: olderCreatedAt));
        await PersistNotification(CreateNotification(eventName: "Concert B", createdAt: newerCreatedAt));
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
}