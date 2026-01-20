using Domain.ValueObjects;

namespace Domain.Notifications;

public readonly struct NotificationType : IEquatable<NotificationType>
{
    private readonly StringValueObject<NotificationType> _value;

    public NotificationType(string type)
    {
        _value = new StringValueObject<NotificationType>(type);
    }

    public override string ToString() => _value.ToString();
    public override bool Equals(object? obj) => obj is NotificationType other && _value.Equals(other._value);
    public bool Equals(NotificationType other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();
    public static bool operator ==(NotificationType left, NotificationType right) => left._value == right._value;
    public static bool operator !=(NotificationType left, NotificationType right) => left._value != right._value;
    public static implicit operator string(NotificationType notificationType) => notificationType._value;
    public static implicit operator NotificationType(string type) => new(type);
}