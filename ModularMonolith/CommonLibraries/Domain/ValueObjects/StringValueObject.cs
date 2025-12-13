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
                errors.Add("Value cannot be null or empty");
            }
        });
        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(StringValueObject<T> valueObject) => valueObject._value;

    public bool Equals(StringValueObject<T> other)
    {
        return string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is StringValueObject<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value?.ToUpperInvariant().GetHashCode() ?? 0;
    }

    public static bool operator ==(StringValueObject<T> left, StringValueObject<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StringValueObject<T> left, StringValueObject<T> right)
    {
        return !left.Equals(right);
    }
}

