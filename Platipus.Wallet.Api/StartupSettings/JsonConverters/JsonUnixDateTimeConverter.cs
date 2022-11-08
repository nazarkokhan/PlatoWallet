namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonUnixDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var milliseconds = reader.GetInt64();
        var result = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        return result.DateTime.ToUniversalTime();
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        var milliseconds = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        var millisecondsString = milliseconds.ToString(CultureInfo.InvariantCulture);
        writer.WriteStringValue(millisecondsString);
    }
}