namespace Testing.Architecture.Tickets.Domain;

public partial class DomainSpecs
{
    [Test]
    public void domain_events_should_be_immutable()
    {
        Given(domain_event_types);
        Then(should_be_immutable);
    }
}