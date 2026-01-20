using Application.Notifications;
using Domain.Notifications;
using Infrastructure.Notifications.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureNotificationsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistNotifications, NotificationRepository>();
        services.AddScoped<GetNotifications>();
        return services;
    }
}