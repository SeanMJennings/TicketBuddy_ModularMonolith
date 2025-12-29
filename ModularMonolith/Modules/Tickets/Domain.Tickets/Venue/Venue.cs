namespace Domain.Tickets.Venue;

public class Venue(Domain.ValueObjects.Venue id, string name, uint capacity)
{
    public Domain.ValueObjects.Venue Id { get; } = id;
    public string Name { get; init; } = name;
    public uint Capacity { get; init; } = capacity;
}