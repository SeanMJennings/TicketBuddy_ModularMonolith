using Domain.Entities;

namespace Domain.Events.Venue;

public class Venue : Entity, IAmAnAggregateRoot
{
    private const uint MinCapacity = 1;
    private const uint MaxCapacity = 50;

    private Venue() : base(Guid.CreateVersion7()) { }

    public Venue(Guid id, VenueName name, Address address, uint capacity) : base(id)
    {
        ValidateCapacity(capacity);

        Name = name;
        Address = address;
        Capacity = capacity;
    }

    public VenueName Name { get; private set; }
    public Address Address { get; private set; }
    public uint Capacity { get; private set; }

    public void UpdateName(VenueName name)
    {
        Name = name;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
    }

    public void UpdateCapacity(uint capacity)
    {
        ValidateCapacity(capacity);
        Capacity = capacity;
    }

    private static void ValidateCapacity(uint capacity)
    {
        Validation.BasedOn(errors =>
        {
            if (capacity < MinCapacity) errors.Add("Capacity must be at least 1");
            if (capacity > MaxCapacity) errors.Add("Capacity cannot exceed 50 seats");
        });
    }
}