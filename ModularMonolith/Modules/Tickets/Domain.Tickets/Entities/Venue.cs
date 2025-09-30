namespace Domain.Tickets.Entities;

public class Venue(Events.Primitives.Venue id, string name, int capacity)
{
    public Events.Primitives.Venue Id { get; } = id;
    public string Name { get; init; } = name;
    public int Capacity { get; init; } = capacity;
}