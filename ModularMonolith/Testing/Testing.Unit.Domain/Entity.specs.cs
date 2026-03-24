using BDD;
using NUnit.Framework;

namespace Domain;

public partial class EntitySpecs : Specification
{
    [Test]
    public void entity_should_not_allow_empty_guid_as_id()
    {
        When(Validating(creating_an_entity_with_empty_guid));
        Then(Informs("Entity ID cannot be an empty GUID."));
    }
}