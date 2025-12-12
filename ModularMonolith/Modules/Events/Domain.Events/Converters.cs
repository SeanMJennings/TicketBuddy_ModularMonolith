using System.Text.Json.Serialization;
using Domain.Events.ValueObjects;

namespace Domain.Events;

public static class EventsConverters
{
    public static List<JsonConverter> GetConverters =>
    [
        new EventNameConverter(),
    ];
}