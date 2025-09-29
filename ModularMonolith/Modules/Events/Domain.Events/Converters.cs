using System.Text.Json.Serialization;
using Domain.Events.Primitives;

namespace Domain.Events;

public static class EventsConverters
{
    public static List<JsonConverter> GetConverters =>
    [
        new EventNameConverter(),
    ];
}