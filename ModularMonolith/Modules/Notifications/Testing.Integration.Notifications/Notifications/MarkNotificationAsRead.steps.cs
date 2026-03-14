using System.Security.Claims;
using Controllers.Notifications;
using Domain.Exceptions;
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

public partial class MarkNotificationAsReadSpecs : TruncateDbSpecification
{
    private MarkNotificationAsReadEndpoint markNotificationAsReadEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private Guid userId = Guid.CreateVersion7();
    private Guid notificationId = Guid.CreateVersion7();

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.CreateVersion7();
        notificationId = Guid.CreateVersion7();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(Setup.Database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<MarkNotificationAsReadEndpoint>()
            .BuildServiceProvider();

        markNotificationAsReadEndpoint = serviceProvider.GetRequiredService<MarkNotificationAsReadEndpoint>();
        AddUserClaimToControllerContext(userId);
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
    }

    private void AddUserClaimToControllerContext(Guid theUserId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", theUserId.ToString())], "TestAuth"));
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

    private Notification CreateNotification(
        Guid? id = null,
        Guid? forUserId = null,
        string eventName = "Concert",
        DateTimeOffset? createdAt = null,
        bool isRead = false)
    {
        var notification = Notification.Create(
            id ?? Guid.CreateVersion7(),
            forUserId ?? userId,
            NotificationType.TicketPurchased,
            $"{{\"eventName\":\"{eventName}\"}}",
            createdAt ?? DateTimeOffset.UtcNow);

        if (isRead)
            notification.MarkAsRead();

        return notification;
    }

    private async Task an_unread_notification_exists()
    {
        await PersistNotification(CreateNotification(id: notificationId));
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

    private static void an_entity_not_found_exception_was_thrown() =>
        error.ShouldBeOfType<EntityNotFoundException>();
}