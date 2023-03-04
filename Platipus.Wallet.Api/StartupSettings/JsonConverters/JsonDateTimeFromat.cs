namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateTimeFormat : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTime = reader.GetDateTime();
        var formatDateTime = dateTime.ToString("dd-MM-yyyy HH:M:ss");
        return DateTime.Parse(formatDateTime);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var dateTimeToWrite = value.ToString("dd-MM-yyyy HH:M:ss");
        writer.WriteStringValue(dateTimeToWrite);
    }
}