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

public partial class GetUnreadCountSpecs : TruncateDbSpecification
{
    private GetUnreadCountEndpoint getUnreadCountEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private Guid userId = Guid.NewGuid();
    private int returnedUnreadCount;

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.NewGuid();
        returnedUnreadCount = 0;

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(Setup.Database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<GetUnreadCountEndpoint>()
            .BuildServiceProvider();

        getUnreadCountEndpoint = serviceProvider.GetRequiredService<GetUnreadCountEndpoint>();
        AddUserClaimToControllerContext(userId);
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
    }

    private void AddUserClaimToControllerContext(Guid theUserId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", theUserId.ToString())], "TestAuth"));
        getUnreadCountEndpoint.ControllerContext = new ControllerContext
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

    private async Task unread_and_read_notifications_exist()
    {
        await PersistNotification(CreateNotification(eventName: "Concert A"));
        await PersistNotification(CreateNotification(eventName: "Concert B"));
        await PersistNotification(CreateNotification(eventName: "Concert C", isRead: true));
    }

    private async Task requesting_unread_count()
    {
        returnedUnreadCount = await getUnreadCountEndpoint.GetUnreadCount();
    }

    private void unread_count_is_returned()
    {
        returnedUnreadCount.ShouldBe(2);
    }
}