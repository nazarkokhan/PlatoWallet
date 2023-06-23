namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class JsonNumberStringAsBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString() == "1";
            case JsonTokenType.True:
            case JsonTokenType.False:
                return reader.GetBoolean();
            default:
                throw new JsonException("Unexpected token type: " + reader.TokenType);
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "true" : "false");
    }
}