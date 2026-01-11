using System.Reflection;
using BDD;
using Domain.DomainEvents;
using Domain.Entities;
using Domain.Tickets.Event;

namespace Testing.Architecture.Tickets.Domain;

internal partial class DomainSpecs : Specification
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
        types = DomainAssembly.GetTypes()
            .Where(t => typeof(IDescribeADomainEvent).IsAssignableFrom(t) && t != typeof(IDescribeADomainEvent));
    }

    private void domain_primitives()
    {
        types = DomainAssembly.GetTypes()
            .Where(t => t.Namespace != null && 
                        t.Namespace.StartsWith("Domain.Tickets") 
                        && !typeof(IDescribeADomainEvent).IsAssignableFrom(t)
                        && t is { IsValueType: true, IsEnum: false });
    }

    private void entity_types_that_are_not_aggregate_roots()
    {
        types = DomainAssembly.GetTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t) &&
                        t != typeof(Entity) &&
                        !typeof(IAmAnAggregateRoot).IsAssignableFrom(t));
    }

    private void entity_types_that_are_aggregate_roots()
    {
        types = DomainAssembly.GetTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t) &&
                        t != typeof(Entity) &&
                        typeof(IAmAnAggregateRoot).IsAssignableFrom(t));
    }

    private void should_be_immutable()
    {
        List<Type> failingTypes = [];
        foreach (var type in types)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var hasWritableField = fields.Any(f => f is { IsInitOnly: false, IsLiteral: false });
            var hasWritableProperty = properties.Any(p => p.CanWrite && !HasInitOnlySetter(p));

            if (hasWritableField || hasWritableProperty)
            {
                failingTypes.Add(type);
            }
        }

        Assert.That(failingTypes, Is.Null.Or.Empty);
    }

    private static bool HasInitOnlySetter(PropertyInfo property)
    {
        var setMethod = property.GetSetMethod(true);
        return setMethod?.ReturnParameter
            .GetRequiredCustomModifiers()
            .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)) == true;
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
        var aggregateRootTypes = DomainAssembly.GetTypes()
            .Where(t => typeof(IAmAnAggregateRoot).IsAssignableFrom(t) && t != typeof(IAmAnAggregateRoot))
            .ToList();

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
