using Domain.Tickets.Core;
using Domain.Tickets.Venue;
using VenueUpserted = Messages.Events.VenueUpserted;

namespace Application.Tickets.Venue;

public class UpsertVenue(
    IPersistVenues venueRepository,
    ITicketsUnitOfWork unitOfWork)
{
    public async Task Execute(VenueUpserted message)
    {
        await venueRepository.Upsert(new Domain.Tickets.Venue.Venue(message.Id, message.Name, message.Capacity));
        await unitOfWork.Commit();
    }
}
