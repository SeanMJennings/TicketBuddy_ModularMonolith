using Domain.Users.Entities;
using Domain.Users.Primitives;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Persistence;

public class UserDbContext(DbContextOptions<UserDbContext> options, DomainEventsDispatcher domainEventsDispatcher) 
    : UnitOfWorkDbContext<UserDbContext>(options, domainEventsDispatcher)
{
    public DbSet<User> Users => Set<User>();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(e => e.Id);
        modelBuilder.Entity<User>().Property(e => e.FullName).HasConversion(name => name.ToString(), name => new FullName(name));
        modelBuilder.Entity<User>().Property(e => e.Email).HasConversion(email => email.ToString(), email => new Email(email));
        modelBuilder.Entity<User>().ToTable("Users","User", e => e.ExcludeFromMigrations());
    }
}
