using Domain.Contracts;
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
        IPersistTickets ticketRepository,
        IUnitOfWork unitOfWork) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            var theVenue = await eventRepository.GetByVenueId(context.Message.Venue);
            var existingEvent = await eventRepository.GetById(context.Message.Id);
            var isNewEvent = existingEvent is null;
            var priceChangedForExistingEvent = existingEvent is not null && existingEvent.Price != context.Message.Price;

            await eventRepository.Save(new Event(context.Message.Id, context.Message.EventName,
                context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));

            if (isNewEvent) await ReleaseTicketsForNewEvent(context, theVenue);
            if (priceChangedForExistingEvent) await UpdateTicketPrices(context);

            await unitOfWork.Commit();
        }

        private async Task UpdateTicketPrices(ConsumeContext<EventUpserted> context)
        {
            var tickets = await ticketRepository.GetByEventId(context.Message.Id);
            
            foreach (var ticket in tickets)
            {
                ticket.UpdatePrice(context.Message.Price);
            }

            await ticketRepository.UpdateRange(tickets);
        }

        private async Task ReleaseTicketsForNewEvent(ConsumeContext<EventUpserted> context, Venue theVenue)
        {
            await TicketsReleaser.ReleaseTicketsForEvent(context.Message.Id, context.Message.Price, theVenue, ticketRepository);
        }
    }
}