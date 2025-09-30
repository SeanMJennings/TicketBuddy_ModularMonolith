namespace Testing.Architecture.Tickets.Domain;

public partial class DomainSpecs
{
    [Test]
    public void domain_events_should_be_immutable()
    {
        Given(domain_event_types);
        Then(should_be_immutable);
    }

    [Test]
    public void entities_that_are_not_aggregate_roots_cannot_be_public()
    {
        Given(entity_types_that_are_not_aggregate_roots);
        Then(should_be_internal_if_not_aggregate_root);
    }

    [Test]
    public void entity_cannot_have_reference_to_other_aggregate_root()
    {
        Given(entity_types_that_are_aggregate_roots);
        Then(should_not_reference_other_aggregate_root);
    }

    [Test]
    public void entity_should_have_parameterless_private_constructor()
    {
        Given(entity_types);
        Then(should_have_parameterless_private_constructor);
    }
}