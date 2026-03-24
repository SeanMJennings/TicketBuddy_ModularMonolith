using BDD;
using NUnit.Framework;

namespace Infrastructure;

public partial class DomainEventsMapperSpecs : Specification
{
    [Test]
    public void returns_handler_when_event_type_is_registered()
    {
        Given(a_mapper_with_a_registered_event_handler);
        When(getting_handler_for_registered_event);
        Then(the_handler_is_returned);
    }

    [Test]
    public void throws_when_no_handler_registered_for_event_type()
    {
        Given(an_empty_mapper);
        When(Validating(getting_handler_for_unregistered_event));
        Then(a_key_not_found_exception_is_thrown);
    }

    [Test]
    public void throws_when_handler_type_is_missing_from_service_provider()
    {
        Given(a_mapper_with_handler_missing_from_service_provider);
        When(Validating(getting_handler_for_registered_event));
        Then(an_invalid_operation_exception_is_thrown);
    }
}