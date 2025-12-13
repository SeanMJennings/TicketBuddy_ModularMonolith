namespace Domain.Tickets.Entities;

public class Venue(Domain.ValueObjects.Venue id, string name, int capacity)
{
    public Domain.ValueObjects.Venue Id { get; } = id;
    public string Name { get; init; } = name;
    public int Capacity { get; init; } = capacity;
}