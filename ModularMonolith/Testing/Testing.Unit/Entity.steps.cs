using Domain.Entities;

namespace Domain;

public partial class EntitySpecs
{
    private class TestEntity(Guid id) : Entity(id);
    private void creating_an_entity_with_empty_guid()
    {
        _ = new TestEntity(Guid.Empty);
    }
}