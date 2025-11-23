namespace Controllers.Tickets.Requests;

public record TicketReservationPayload(Guid[] ticketIds);
public record TicketPurchasePayload(Guid[] ticketIds);