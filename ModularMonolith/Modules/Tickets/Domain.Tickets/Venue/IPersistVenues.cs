namespace Domain.Tickets.Venue;

public interface IPersistVenues
{
    Task Upsert(Venue venue);
    Task<Venue?> GetById(Guid id);
}
