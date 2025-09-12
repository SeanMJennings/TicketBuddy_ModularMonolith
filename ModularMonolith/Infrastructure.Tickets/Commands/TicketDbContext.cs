using Domain.Tickets.Entities;
using Domain.Tickets.Primitives;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;

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
        modelBuilder.Entity<Event>().ToTable("Events",DefaultSchema, e => e.ExcludeFromMigrations());
        
        modelBuilder.Entity<Ticket>().HasKey(t => t.Id);
        modelBuilder.Entity<Ticket>().Property(t => t.SeatNumber).HasConversion(seat => (int)seat, seat => (uint)seat);
        modelBuilder.Entity<Ticket>().ToTable("Tickets",DefaultSchema, t => t.ExcludeFromMigrations());
        
        modelBuilder.Entity<Venue>().HasKey(v => v.Id);
        modelBuilder.Entity<Venue>().ToTable("EventVenues",DefaultSchema, v => v.ExcludeFromMigrations());
        
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.FullName).HasConversion(name => name.ToString(), name => new Name(name));
        modelBuilder.Entity<User>().Property(u => u.Email).HasConversion(email => email.ToString(), email => new Email(email));
        modelBuilder.Entity<User>().ToTable("Users",DefaultSchema, u => u.ExcludeFromMigrations());
    }
}
