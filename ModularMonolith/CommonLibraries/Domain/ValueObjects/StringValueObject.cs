using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public readonly struct StringValueObject<T> : IEquatable<StringValueObject<T>>
    where T : struct
{
    private readonly string _value;
    
    public StringValueObject(string value)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add($"{typeof(T).Name} cannot be null or empty");
            }
        });
        _value = value;
    }

    public override string ToString() => _value;
    public static implicit operator string(StringValueObject<T> valueObject) => valueObject._value;
    public bool Equals(StringValueObject<T> other) => string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is StringValueObject<T> other && Equals(other);
    public override int GetHashCode() => _value?.ToUpperInvariant().GetHashCode() ?? 0;
    public static bool operator ==(StringValueObject<T> left, StringValueObject<T> right) => left.Equals(right);
    public static bool operator !=(StringValueObject<T> left, StringValueObject<T> right) => !left.Equals(right);
}

public abstract class StringValueObjectJsonConverter<T> : JsonConverter<T> where T : struct
{
    protected abstract T CreateFromString(string value);

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return CreateFromString(reader.GetString()!);
    }
}