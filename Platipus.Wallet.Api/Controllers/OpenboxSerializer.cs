namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using System.Text.Json.Serialization;
using JorgeSerrano.Json;
using StartupSettings.JsonConverters;

//TODO put to DI
public static class OpenboxSerializer
{
    public static readonly JsonSerializerOptions Value = new()
    {
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
        Converters =
        {
            new JsonStringEnumConverter(),
            new JsonUnixDateTimeConverter()
        },
        NumberHandling = JsonNumberHandling.WriteAsString
                       | JsonNumberHandling.AllowReadingFromString
    };
}