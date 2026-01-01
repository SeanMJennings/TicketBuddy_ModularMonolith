namespace Controllers.Events;

public static class Routes
{
    public const string Events = "events";
    public const string TheEvent = $"{Events}/{{id:guid}}";
    public const string Venues = "venues";
    public const string TheVenue = $"{Venues}/{{id:guid}}";
}