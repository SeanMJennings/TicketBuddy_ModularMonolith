using NUnit.Framework;

namespace Component.Api;

public partial class HealthApiSpecs
{
    [Test]
    public void health_check_returns_ok()
    {
        //Given(a_postgresql_database_is_available);
        And(a_redis_cache_is_available);
        And(a_rabbitmq_broker_is_available);
        And(the_api_is_running);
        When(calling_the_health_endpoint);
        Then(the_response_is_ok);
    }
}