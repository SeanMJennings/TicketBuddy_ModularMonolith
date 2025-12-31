namespace Controllers.Tickets.Requests;

public record TicketPurchasePayload(Guid[] ticketIds);