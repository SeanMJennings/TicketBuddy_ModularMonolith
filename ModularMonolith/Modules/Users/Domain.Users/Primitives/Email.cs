using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Users.Primitives;

[JsonConverter(typeof(EmailConverter))]
public readonly record struct Email
{
    private string value { get; }

    public Email(string email)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrEmpty(email))
            {
                errors.Add("Email cannot be empty");
            }
            else if (!Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Email must be valid");
            }
        });
        value = email;
    }
    
    public override string ToString()
    {
        return value;
    }

    public static implicit operator string(Email email) => email.value;
    
    public static implicit operator Email(string email) => new(email);
}

public class EmailConverter : JsonConverter<Email>
{
    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }
}