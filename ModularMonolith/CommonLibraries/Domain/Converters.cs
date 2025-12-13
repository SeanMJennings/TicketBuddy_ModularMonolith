using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace Domain;

public static class Converters
{
    public static List<JsonConverter> GetConverters =>
    [
        new EventNameConverter(),
        new MoneyConverter()
    ];
}