using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public readonly record struct Money
{
    private decimal Amount { get; }
    
    public Money(decimal amount)
    {
        Validation.BasedOn((errors) =>
        {
            if (amount < 0)
            {
                errors.Add("Amount cannot be negative.");
            }
        });

        Amount = amount;
    }

    public override string ToString()
    {
        return Amount.ToString("F");
    }
    
    public static implicit operator decimal(Money money) => money.Amount;
    
    public static implicit operator Money(decimal amount) => new(amount);
}

public class MoneyConverter : JsonConverter<Money>
{
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }

    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }
}