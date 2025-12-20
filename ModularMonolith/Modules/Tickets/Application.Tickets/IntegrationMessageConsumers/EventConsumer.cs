using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Domain.Tickets.Services;
using MassTransit;
using Event = Domain.Tickets.Entities.Event;
using EventUpserted = Integration.Events.Messaging.EventUpserted;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumer(
        IPersistEvents eventRepository,
        IPersistTickets ticketRepository) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            var theVenue = await eventRepository.GetByVenueId(context.Message.Venue);
            var existingEvent = await eventRepository.GetById(context.Message.Id);
            var isNewEvent = existingEvent is null;
            var priceChanged = existingEvent is not null && existingEvent.Price != context.Message.Price;

            await eventRepository.Save(new Event(context.Message.Id, context.Message.EventName,
                context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));

            if (await ReleaseTicketsIfEventIsNew(context, isNewEvent, theVenue)) return;
            if (priceChanged) await UpdateTicketPrice(context);

            await eventRepository.Commit();
        }

        private async Task UpdateTicketPrice(ConsumeContext<EventUpserted> context)
        {
            var tickets = await ticketRepository.GetByEventId(context.Message.Id);
            foreach (var ticket in tickets)
            {
                ticket.UpdatePrice(context.Message.Price);
            }

            await ticketRepository.UpdateRange(tickets);
            await ticketRepository.Commit();
        }

        private async Task<bool> ReleaseTicketsIfEventIsNew(ConsumeContext<EventUpserted> context, bool isNewEvent, Venue theVenue)
        {
            if (!isNewEvent) return false;
            await TicketsReleaseService.ReleaseTicketsForEvent(context.Message.Id, context.Message.Price, theVenue, ticketRepository);
            await ticketRepository.Commit();
            await eventRepository.Commit();
            return true;
        }
    }
}