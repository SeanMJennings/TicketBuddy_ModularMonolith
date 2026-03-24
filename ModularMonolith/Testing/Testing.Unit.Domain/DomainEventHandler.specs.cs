using BDD;
using NUnit.Framework;

namespace Domain;

public partial class DomainEventHandlerSpecs : Specification
{
    [Test]
    public void handler_processes_the_correct_event_type()
    {
        Given(a_handler_registered_for_an_event_type);
        When(dispatching_the_correct_event_type);
        Then(the_handler_processes_the_event);
    }

    [Test]
    public void dispatching_the_wrong_event_type_throws()
    {
        Given(a_handler_registered_for_an_event_type);
        When(Validating(dispatching_the_wrong_event_type));
        Then(Informs("Message is not of the expected type (Parameter 'message')"));
    }
}