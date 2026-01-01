namespace Domain.Events.Venue;

public interface IPersistVenues
{
    Task<IEnumerable<Venue>> GetAll();
    void Add(Venue venue);
    Task<Venue?> GetById(Guid id);
}