using Domain.Entities;

namespace Domain.Tickets.Venue;

public class Venue(Guid id, string name, uint capacity) : Entity(id), IAmAnAggregateRoot
{
    public string Name { get; init; } = name;
    public uint Capacity { get; init; } = capacity;
}