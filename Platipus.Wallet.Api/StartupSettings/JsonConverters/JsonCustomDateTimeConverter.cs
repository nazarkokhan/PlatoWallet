namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonCustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        var result = DateTime.ParseExact(
            dateString!,
            DateTimeFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None).ToUniversalTime();
        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        Span<char> destination = stackalloc char[19];

        var success = value.TryFormat(
            destination,
            out _,
            DateTimeFormat,
            CultureInfo.InvariantCulture);

        if (success)
            writer.WriteStringValue(destination);
        else
            throw new JsonException("Failed to format DateTime.");
    }
}