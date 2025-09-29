namespace Domain.Tickets.Entities;

public class Venue
{
    public Venue(Events.Primitives.Venue Id, string Name, int Capacity)
    {
        this.Id = Id;
        this.Name = Name;
        this.Capacity = Capacity;
    }
    private Venue() {}
    public Events.Primitives.Venue Id { get; }
    public string Name { get; init; }
    public int Capacity { get; init; }
}