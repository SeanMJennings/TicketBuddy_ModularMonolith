using Domain.ValueObjects;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Core;

public class EventDbContext(
    DbContextOptions<EventDbContext> options,
    DomainEventsDispatcher domainEventsDispatcher,
    IOutboxFlusher outboxFlusher)
    : UnitOfWorkDbContext<EventDbContext>(options, domainEventsDispatcher, outboxFlusher)
{
    public DbSet<Domain.Events.Event> Events => Set<Domain.Events.Event>();
    public DbSet<Domain.Events.Venue.Venue> Venues => Set<Domain.Events.Venue.Venue>();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Events.Event>().HasKey(e => e.Id);
        modelBuilder.Entity<Domain.Events.Event>().Property(e => e.EventName).HasConversion(name => name.ToString(), name => new EventName(name));
        modelBuilder.Entity<Domain.Events.Event>().Property(e => e.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount));
        modelBuilder.Entity<Domain.Events.Event>().Property(e => e.VenueId).HasColumnName("Venue");
        modelBuilder.Entity<Domain.Events.Event>().ToTable("Events","Event", e => e.ExcludeFromMigrations());

        modelBuilder.Entity<Domain.Events.Venue.Venue>().HasKey(v => v.Id);
        modelBuilder.Entity<Domain.Events.Venue.Venue>().Property(v => v.Name).HasConversion(name => name.ToString(), name => new Domain.Events.Venue.VenueName(name));
        modelBuilder.Entity<Domain.Events.Venue.Venue>().ComplexProperty(v => v.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).IsRequired();
            addressBuilder.Property(a => a.City).IsRequired();
            addressBuilder.Property(a => a.Postcode).IsRequired();
        });
        modelBuilder.Entity<Domain.Events.Venue.Venue>().ToTable("Venues", "Event", e => e.ExcludeFromMigrations());
    }
}