using BDD;
using NUnit.Framework;

namespace Infrastructure;

public partial class DatabaseSpecs : Specification
{
    [Test]
    public void throws_when_connection_string_is_null()
    {
        Given(a_null_connection_string);
        When(Validating(creating_a_database));
        Then(Informs("A connection string is required."));
    }

    [Test]
    public void throws_when_connection_string_is_empty()
    {
        Given(an_empty_connection_string);
        When(Validating(creating_a_database));
        Then(Informs("A connection string is required."));
    }
}