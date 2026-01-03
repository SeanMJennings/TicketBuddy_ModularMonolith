namespace Domain.Tickets.Venue;

public class Venue(Guid id, string name, uint capacity)
{
    public Guid Id { get; } = id;
    public string Name { get; init; } = name;
    public uint Capacity { get; init; } = capacity;
}