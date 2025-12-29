using Domain.ValueObjects;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Core;

public class EventDbContext(DbContextOptions<EventDbContext> options, DomainEventsDispatcher domainEventsDispatcher) 
    : UnitOfWorkDbContext<EventDbContext>(options, domainEventsDispatcher)
{
    public DbSet<Domain.Events.Entities.Event> Events => Set<Domain.Events.Entities.Event>();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Events.Entities.Event>().HasKey(e => e.Id);
        modelBuilder.Entity<Domain.Events.Entities.Event>().Property(e => e.EventName).HasConversion(name => name.ToString(), name => new EventName(name));
        modelBuilder.Entity<Domain.Events.Entities.Event>().Property(e => e.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount));
        modelBuilder.Entity<Domain.Events.Entities.Event>().ToTable("Events","Event", e => e.ExcludeFromMigrations());
    }
}

