﻿namespace Controllers.Tickets;

public static class Routes
{
    public const string Events = "events";
    public const string Tickets = $"{Events}/{{id:guid}}/tickets";
    public const string TicketsPurchase = $"{Events}/{{id:guid}}/tickets/purchase";
    public static string EventTickets(Guid id) => $"{Events}/{id}/tickets";
}