using Domain.Tickets.User;
using Domain.ValueObjects;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Core;

public class TicketDbContext(DbContextOptions<TicketDbContext> options, DomainEventsDispatcher domainEventsDispatcher)
    : UnitOfWorkDbContext<TicketDbContext>(options, domainEventsDispatcher)
{
    private const string DefaultSchema = "Ticket";
    public DbSet<Domain.Tickets.Event.Event> Events => Set<Domain.Tickets.Event.Event>();
    public DbSet<Domain.Tickets.Ticket.Ticket> Tickets => Set<Domain.Tickets.Ticket.Ticket>();
    public DbSet<Domain.Tickets.Venue.Venue> Venues => Set<Domain.Tickets.Venue.Venue>();
    public DbSet<Domain.Tickets.User.User> Users => Set<Domain.Tickets.User.User>();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Tickets.Event.Event>().HasKey(e => e.Id);
        modelBuilder.Entity<Domain.Tickets.Event.Event>().Property(e => e.EventName).HasConversion(name => name.ToString(), name => new EventName(name));
        modelBuilder.Entity<Domain.Tickets.Event.Event>().Property(e => e.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount));
        modelBuilder.Entity<Domain.Tickets.Event.Event>().Property(e => e.VenueId).HasColumnName("Venue");
        modelBuilder.Entity<Domain.Tickets.Event.Event>().ToTable("Events",DefaultSchema, e => e.ExcludeFromMigrations());
        
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().HasKey(t => t.Id);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().Property(t => t.EventId);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().Property(t => t.Price).HasConversion(amount => (decimal)amount, amount => new Money(amount)).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().Property(t => t.SeatNumber).HasConversion(seat => (int)seat, seat => (uint)seat);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().Property(t => t.PurchasedAt).IsRequired(false);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().Property(t => t.UserId).IsRequired(false);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().HasIndex(t => t.EventId);
        modelBuilder.Entity<Domain.Tickets.Ticket.Ticket>().ToTable("Tickets",DefaultSchema, t => t.ExcludeFromMigrations());
        
        modelBuilder.Entity<Domain.Tickets.Venue.Venue>().HasKey(v => v.Id);
        modelBuilder.Entity<Domain.Tickets.Venue.Venue>().ToTable("EventVenues",DefaultSchema, v => v.ExcludeFromMigrations());
        
        modelBuilder.Entity<Domain.Tickets.User.User>().HasKey(u => u.Id);
        modelBuilder.Entity<Domain.Tickets.User.User>().Property(u => u.FullName).HasConversion(name => name.ToString(), name => new Name(name));
        modelBuilder.Entity<Domain.Tickets.User.User>().Property(u => u.Email).HasConversion(email => email.ToString(), email => new Email(email));
        modelBuilder.Entity<Domain.Tickets.User.User>().ToTable("Users",DefaultSchema, u => u.ExcludeFromMigrations());
    }
}