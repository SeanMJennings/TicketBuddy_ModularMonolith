namespace Domain.Events.Venue;

public interface IPersistVenues
{
    Task<IEnumerable<Venue>> GetAll();
    Task Add(Venue venue);
    Task<Venue?> GetById(Guid id);
    Task Update(Venue venue);
}