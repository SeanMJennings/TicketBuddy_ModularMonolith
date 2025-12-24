using Domain.Tickets.Entities;
using Domain.Tickets.ValueObjects;
using Domain.ValueObjects;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;
using Venue = Domain.Tickets.Entities.Venue;

namespace Infrastructure.Tickets.Commands;

public class TicketDbContext(DbContextOptions<TicketDbContext> options, DomainEventsDispatcher domainEventsDispatcher) 
    : UnitOfWorkDbContext<TicketDbContext>(options, domainEventsDispatcher)
{
    private const string DefaultSchema = "Ticket";
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<User> Users => Set<User>();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().HasKey(e => e.Id);
        modelBuilder.Entity<Event>().Property(e => e.EventName).HasConversion(name => name.ToString(), name => new EventName(name));
        modelBuilder.Entity<Event>().Property(e => e.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount));
        modelBuilder.Entity<Event>().Property(e => e.Venue).HasColumnName("Venue");
        modelBuilder.Entity<Event>().ToTable("Events",DefaultSchema, e => e.ExcludeFromMigrations());
        
        modelBuilder.Entity<Ticket>().HasKey(t => t.Id);
        modelBuilder.Entity<Ticket>().Property(t => t.EventId);
        modelBuilder.Entity<Ticket>().Property(t => t.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount)).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Ticket>().Property(t => t.SeatNumber).HasConversion(seat => (int)seat, seat => (uint)seat);
        modelBuilder.Entity<Ticket>().Property(t => t.PurchasedAt).IsRequired(false);
        modelBuilder.Entity<Ticket>().Property(t => t.UserId).IsRequired(false);
        modelBuilder.Entity<Ticket>().HasIndex(t => t.EventId);
        modelBuilder.Entity<Ticket>().ToTable("Tickets",DefaultSchema, t => t.ExcludeFromMigrations());
        
        modelBuilder.Entity<Venue>().HasKey(v => v.Id);
        modelBuilder.Entity<Venue>().ToTable("EventVenues",DefaultSchema, v => v.ExcludeFromMigrations());
        
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.FullName).HasConversion(name => name.ToString(), name => new Name(name));
        modelBuilder.Entity<User>().Property(u => u.Email).HasConversion(email => email.ToString(), email => new Email(email));
        modelBuilder.Entity<User>().ToTable("Users",DefaultSchema, u => u.ExcludeFromMigrations());
    }
}
