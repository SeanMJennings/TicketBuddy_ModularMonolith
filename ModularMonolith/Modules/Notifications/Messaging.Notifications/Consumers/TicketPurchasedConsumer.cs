using Application.Notifications;
using MassTransit;
using Messages.Tickets;

namespace Messaging.Notifications.Consumers;

public class TicketPurchasedConsumer(
    CreateTicketPurchaseNotification createNotification) : IConsumer<TicketPurchased>
{
    public async Task Consume(ConsumeContext<TicketPurchased> context)
    {
        var message = context.Message;
        await createNotification.Execute(
            message.UserId,
            message.TicketId,
            message.EventId,
            message.EventName);
    }
}

public class TicketPurchasedConsumerDefinition : ConsumerDefinition<TicketPurchasedConsumer>
{
    public TicketPurchasedConsumerDefinition()
    {
        EndpointName = "notifications-queue";
    }
}