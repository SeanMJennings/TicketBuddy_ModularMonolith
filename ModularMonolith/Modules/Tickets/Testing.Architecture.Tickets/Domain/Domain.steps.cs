using System.Reflection;
using BDD;
using Domain.DomainEvents;
using Domain.Tickets.Entities;
using NetArchTest.Rules;

namespace Testing.Architecture.Tickets.Domain;

public partial class DomainSpecs : Specification
{
    private IEnumerable<Type> types = [];
    private static Assembly DomainAssembly => typeof(Event).Assembly;
    protected override void before_each()
    {
        base.before_each();
        types = new List<Type>();
    }

    private void domain_event_types()
    {
        types = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(IAmADomainEvent))
            .GetTypes();
    }

    private void should_be_immutable()
    {
        List<Type> failingTypes = [];
        foreach (var type in types)
        {
            if (type.GetFields().All(x => x.IsInitOnly) && !type.GetProperties().Any(x => x.CanWrite)) continue;
            failingTypes.Add(type);
            break;
        }

        Assert.That(failingTypes, Is.Null.Or.Empty);
    }
}