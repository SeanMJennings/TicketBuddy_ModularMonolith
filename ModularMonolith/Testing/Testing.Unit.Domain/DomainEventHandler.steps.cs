using Domain.DomainEvents;
using Shouldly;

namespace Domain;

public partial class DomainEventHandlerSpecs
{
    private IHandleDomainEvents handler = null!;
    private bool handlerWasCalled;

    private void a_handler_registered_for_an_event_type() => handler = new SampleDomainEventHandler();

    private void dispatching_the_correct_event_type()
    {
        handler.Handle(new SampleDomainEvent()).GetAwaiter().GetResult();
        handlerWasCalled = true;
    }

    private void dispatching_the_wrong_event_type() => handler.Handle(new OtherDomainEvent()).GetAwaiter().GetResult();

    private void the_handler_processes_the_event() => handlerWasCalled.ShouldBeTrue();

    private record SampleDomainEvent : IDescribeADomainEvent;
    private record OtherDomainEvent : IDescribeADomainEvent;

    private class SampleDomainEventHandler : HandleDomainEvents<SampleDomainEvent>
    {
        protected override Task Handle(SampleDomainEvent message) => Task.CompletedTask;
    }
}