namespace Controllers.Tickets;

public static class Routes
{
    public const string Events = "events";
    public const string Tickets = $"{Events}/{{id:guid}}/tickets";
    public const string TicketsPurchase = $"{Events}/{{id:guid}}/tickets/purchase";
    public const string TicketsReservation = $"{Events}/{{id:guid}}/tickets/reserve";
    public const string TicketsPurchased = $"{Events}/{{id:guid}}/tickets/user/{{userId:guid}}";
}