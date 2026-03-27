using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options) : DbContext(options)
{
    private const string Schema = "Messaging";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity(b => b.ToTable("InboxState", Schema, t => t.ExcludeFromMigrations()));
        modelBuilder.AddOutboxStateEntity(b => b.ToTable("OutboxState", Schema, t => t.ExcludeFromMigrations()));
        modelBuilder.AddOutboxMessageEntity(b => b.ToTable("OutboxMessage", Schema, t => t.ExcludeFromMigrations()));
    }
}