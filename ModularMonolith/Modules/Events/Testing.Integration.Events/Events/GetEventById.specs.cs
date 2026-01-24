using NUnit.Framework;

namespace Integration.Events;

public partial class GetEventByIdSpecs
{
    [Test]
    public async Task get_event_returns_not_found_for_non_existent_event()
    {
        await When(requesting_a_non_existent_event);
              Then(a_not_found_response_is_returned);
    }
}