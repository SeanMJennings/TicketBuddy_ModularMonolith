using Application.Tickets.Venue;
using MassTransit;
using VenueUpserted = Messages.Events.VenueUpserted;

namespace Messaging.Tickets.Consumers
{
    public class VenueUpsertedConsumer(UpsertVenue upsertVenue) : IConsumer<VenueUpserted>
    {
        public async Task Consume(ConsumeContext<VenueUpserted> context)
        {
            await upsertVenue.Execute(context.Message);
        }
    }
}