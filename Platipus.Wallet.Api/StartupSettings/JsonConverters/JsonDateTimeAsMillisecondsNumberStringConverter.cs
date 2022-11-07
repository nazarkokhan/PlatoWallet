namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateTimeAsMillisecondsNumberStringConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var milliseconds = reader.GetInt32();
        var result = new DateTime();
        result = result.AddMilliseconds(milliseconds);
        return result;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        var milliseconds = TimeSpan.FromTicks(value.Ticks).TotalMilliseconds;
        var millisecondsString = milliseconds.ToString(CultureInfo.InvariantCulture);
        writer.WriteStringValue(millisecondsString);
    }
}