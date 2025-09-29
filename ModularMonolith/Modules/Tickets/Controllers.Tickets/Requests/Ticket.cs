namespace Controllers.Tickets.Requests;

public record TicketReservationPayload(Guid userId, Guid[] ticketIds);
public record TicketPurchasePayload(Guid userId, Guid[] ticketIds);