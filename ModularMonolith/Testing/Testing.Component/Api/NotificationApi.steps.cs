using System.Net;
using System.Net.Http.Json;
using Application;
using Controllers.Notifications;
using Domain.Notifications;
using Infrastructure.Notifications.Core;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

namespace Component.Api;

public partial class NotificationApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private static PostgreSqlContainer database = null!;

    private Guid userId = Guid.CreateVersion7();
    private Guid notificationId = Guid.CreateVersion7();
    private HttpStatusCode responseCode;
    private List<NotificationResponse> returnedNotifications = [];
    private int returnedUnreadCount;

    protected override async Task before_all()
    {
        database = await SharedContainers.GetPostgreSqlAsync();
    }

    protected override Task before_each()
    {
        userId = Guid.CreateVersion7();
        notificationId = Guid.CreateVersion7();
        returnedNotifications = [];
        returnedUnreadCount = 0;

        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Customer));
        client.DefaultRequestHeaders.Add(UserHeaders.UserId, userId.ToString());
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        client.Dispose();
        await factory.DisposeAsync();
    }

    protected override Task after_all() => Task.CompletedTask;

    private async Task a_notification_exists_for_the_user()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPersistNotifications>();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

        var notification = Notification.Create(
            notificationId,
            userId,
            NotificationType.TicketPurchased,
            "{\"eventName\":\"Test Concert\"}",
            DateTimeOffset.UtcNow);

        await repository.Add(notification);
        await dbContext.Commit();
    }

    private async Task requesting_notifications_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
        var response = await client.GetAsync(Routes.Notifications);
        responseCode = response.StatusCode;
    }

    private async Task marking_notification_as_read_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
        var response = await client.PostAsync($"notifications/{notificationId}/read", null);
        responseCode = response.StatusCode;
    }

    private async Task requesting_unread_count_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
        var response = await client.GetAsync(Routes.UnreadCount);
        responseCode = response.StatusCode;
    }

    private async Task requesting_notifications()
    {
        var response = await client.GetAsync(Routes.Notifications);
        responseCode = response.StatusCode;
        if (response.IsSuccessStatusCode)
            returnedNotifications = await response.Content.ReadFromJsonAsync<List<NotificationResponse>>(JsonSerialization.GetJsonSerializerOptions()) ?? [];
    }

    private async Task marking_notification_as_read()
    {
        var response = await client.PostAsync($"notifications/{notificationId}/read", null);
        responseCode = response.StatusCode;
    }

    private async Task requesting_unread_count()
    {
        var response = await client.GetAsync(Routes.UnreadCount);
        responseCode = response.StatusCode;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            returnedUnreadCount = int.Parse(content);
        }
    }

    private void the_request_is_unauthorized()
    {
        responseCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private void the_notifications_are_returned()
    {
        responseCode.ShouldBe(HttpStatusCode.OK);
        returnedNotifications.Count.ShouldBe(1);
        returnedNotifications[0].Id.ShouldBe(notificationId);
    }

    private void the_request_succeeds()
    {
        responseCode.ShouldBe(HttpStatusCode.NoContent);
    }

    private void the_unread_count_is_returned()
    {
        responseCode.ShouldBe(HttpStatusCode.OK);
        returnedUnreadCount.ShouldBe(1);
    }
}