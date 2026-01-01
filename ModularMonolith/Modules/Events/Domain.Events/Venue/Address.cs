using System.Text.RegularExpressions;

namespace Domain.Events.Venue;

public readonly struct Address : IEquatable<Address>
{
    private readonly string _street;
    private readonly string _city;
    private readonly string _postcode;

    public Address(string street, string city, string postcode)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrWhiteSpace(street)) errors.Add("Street is required");
            if (string.IsNullOrWhiteSpace(city)) errors.Add("City is required");
            if (!IsValidUkPostcode(postcode)) errors.Add("Invalid UK postcode format");
        });

        _street = street;
        _city = city;
        _postcode = postcode.ToUpperInvariant();
    }

    public string Street => _street;
    public string City => _city;
    public string Postcode => _postcode;

    private static bool IsValidUkPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode)) return false;

        const string pattern = @"^[A-Z]{1,2}[0-9]{1,2}[A-Z]?\s?[0-9][A-Z]{2}$";
        return Regex.IsMatch(
            postcode.ToUpperInvariant(),
            pattern,
            RegexOptions.None,
            TimeSpan.FromMilliseconds(100));
    }

    public bool Equals(Address other) =>
        string.Equals(_street, other._street, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(_city, other._city, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(_postcode, other._postcode, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Address other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        _street?.ToUpperInvariant(),
        _city?.ToUpperInvariant(),
        _postcode?.ToUpperInvariant());

    public static bool operator ==(Address left, Address right) => left.Equals(right);
    public static bool operator !=(Address left, Address right) => !left.Equals(right);

    public override string ToString() => $"{_street}, {_city}, {_postcode}";
}