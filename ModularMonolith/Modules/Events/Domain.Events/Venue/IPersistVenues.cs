namespace Domain.Events.Venue;

public interface IPersistVenues
{
    Task<IEnumerable<Venue>> GetAll();
}