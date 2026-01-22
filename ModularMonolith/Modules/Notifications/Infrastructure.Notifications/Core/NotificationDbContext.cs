using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Notifications.Core;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options, DomainEventsDispatcher domainEventsDispatcher)
    : UnitOfWorkDbContext<NotificationDbContext>(options, domainEventsDispatcher)
{
    private const string DefaultSchema = "Notification";

    public DbSet<Domain.Notifications.Notification> Notifications => Set<Domain.Notifications.Notification>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Notifications.Notification>().HasKey(n => n.Id);
        modelBuilder.Entity<Domain.Notifications.Notification>().Property(n => n.UserId);
        modelBuilder.Entity<Domain.Notifications.Notification>().Property(n => n.Type)
            .HasConversion<string>();
        modelBuilder.Entity<Domain.Notifications.Notification>().Property(n => n.Payload).HasColumnType("jsonb");
        modelBuilder.Entity<Domain.Notifications.Notification>().Property(n => n.IsRead);
        modelBuilder.Entity<Domain.Notifications.Notification>().Property(n => n.CreatedAt);
        modelBuilder.Entity<Domain.Notifications.Notification>().HasIndex(n => n.UserId);
        modelBuilder.Entity<Domain.Notifications.Notification>().HasIndex(n => new { n.UserId, n.IsRead });
        modelBuilder.Entity<Domain.Notifications.Notification>().ToTable("Notifications", DefaultSchema, n => n.ExcludeFromMigrations());
    }
}