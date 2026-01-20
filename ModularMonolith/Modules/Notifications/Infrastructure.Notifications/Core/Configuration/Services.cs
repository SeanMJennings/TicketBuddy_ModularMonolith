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
        services.AddScoped<INotificationsUnitOfWork, UnitOfWork>();
        services.AddScoped<GetNotifications>();
        services.AddScoped<MarkNotificationAsRead>();
        services.AddScoped<GetUnreadCount>();
        return services;
    }
}