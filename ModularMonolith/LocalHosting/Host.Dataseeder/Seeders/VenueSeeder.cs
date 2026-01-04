using Controllers.Events.Requests;
using Dataseeder.Infrastructure;
using Dataseeder.SeedData;

namespace Dataseeder.Seeders;

internal class VenueSeeder(TicketBuddyApiClient apiClient)
{
    internal async Task SeedAsync()
    {
        foreach (var venue in VenueSeedData.Venues)
        {
            var payload = new VenuePayload(
                venue.Name,
                venue.Street,
                venue.City,
                venue.Postcode,
                venue.Capacity);

            await apiClient.CreateVenueAsync(payload);
        }
    }

    internal async Task<Dictionary<string, Guid>> GetVenueIdsAsync()
    {
        var venues = await apiClient.GetVenuesAsync();
        var venueIds = new Dictionary<string, Guid>();

        foreach (var venue in VenueSeedData.Venues)
        {
            var existingVenue = venues.FirstOrDefault(v => v.Name == venue.Name);
            if (existingVenue != null)
            {
                venueIds[venue.Key] = existingVenue.Id;
            }
        }

        return venueIds;
    }
}