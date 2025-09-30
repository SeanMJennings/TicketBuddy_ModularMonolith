using System.Reflection;
using BDD;
using Domain;
using Domain.DomainEvents;
using Domain.Events.Entities;
using NetArchTest.Rules;

namespace Testing.Architecture.Events.Domain;

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

    private void domain_primitives()
    {
        types = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("Domain.Events.Primitives")
            .And()
            .DoNotInherit(typeof(Enum))
            .GetTypes();
    }

    private void entity_types_that_are_not_aggregate_roots()
    {
        types = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .And()
            .DoNotImplementInterface(typeof(IAmAnAggregateRoot))
            .GetTypes();
    }

    private void entity_types_that_are_aggregate_roots()
    {
        types = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .And()
            .ImplementInterface(typeof(IAmAnAggregateRoot))
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
    
    private void should_not_be_public_if_not_aggregate_root()
    {
        List<Type> failingTypes = [];
        foreach (var type in types)
        {
            if (type.IsNotPublic) continue;
            failingTypes.Add(type);
            break;
        }

        Assert.That(failingTypes, Is.Null.Or.Empty);
    }

    private void should_not_reference_other_aggregate_root()
    {
        var aggregateRootTypes = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IAmAnAggregateRoot))
            .GetTypes().ToList();

        List<Type> failingTypes = [];
        foreach (var entityType in types)
        {
            var fields = entityType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (fields.Any(f => aggregateRootTypes.Contains(f.FieldType)) ||
                properties.Any(p => aggregateRootTypes.Contains(p.PropertyType)))
            {
                failingTypes.Add(entityType);
            }
        }

        Assert.That(failingTypes, Is.Null.Or.Empty);
    }
}