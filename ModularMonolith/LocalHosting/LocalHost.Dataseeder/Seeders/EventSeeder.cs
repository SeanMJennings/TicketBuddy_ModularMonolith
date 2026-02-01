using Controllers.Events.Requests;
using Dataseeder.Infrastructure;
using Dataseeder.SeedData;

namespace Dataseeder.Seeders;

internal class EventSeeder(TicketBuddyApiClient apiClient)
{
    internal async Task SeedAsync(Dictionary<string, Guid> venueIds)
    {
        foreach (var eventData in EventSeedData.GetEvents())
        {
            var payload = new EventPayload(
                eventData.Name,
                eventData.StartDate,
                eventData.EndDate,
                venueIds[eventData.VenueKey],
                eventData.Price);

            await apiClient.CreateEventAsync(payload);
        }
    }
}