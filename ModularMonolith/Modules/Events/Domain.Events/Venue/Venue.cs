using Domain.Entities;

namespace Domain.Events.Venue;

public class Venue : Entity, IAmAnAggregateRoot
{
    private const uint MinCapacity = 1;
    private const uint MaxCapacity = 50;

    public Venue(Guid id, VenueName name, Address address, uint capacity) : base(id)
    {
        Validation.BasedOn(errors =>
        {
            if (capacity < MinCapacity)
                errors.Add("Capacity must be at least 1");
            if (capacity > MaxCapacity)
                errors.Add("Capacity cannot exceed 50 seats");
        });

        Name = name;
        Address = address;
        Capacity = capacity;
    }

    public VenueName Name { get; private set; }
    public Address Address { get; private set; }
    public uint Capacity { get; private set; }
}
