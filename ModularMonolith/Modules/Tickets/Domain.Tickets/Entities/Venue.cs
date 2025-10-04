namespace Domain.Tickets.Entities;

public class Venue(Domain.Primitives.Venue id, string name, int capacity)
{
    public Domain.Primitives.Venue Id { get; } = id;
    public string Name { get; init; } = name;
    public int Capacity { get; init; } = capacity;
}