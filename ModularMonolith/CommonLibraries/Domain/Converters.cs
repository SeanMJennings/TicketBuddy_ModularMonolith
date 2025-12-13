using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace Domain;

public static class EventsConverters
{
    public static List<JsonConverter> GetConverters =>
    [
        new EventNameConverter(),
    ];
}