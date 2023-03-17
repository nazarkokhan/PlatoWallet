namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.ComponentModel;
using System.Text.Json.Serialization;

public record ReevoAddFreeRoundsGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    string Title,
    [property: JsonPropertyName("playerids")] string PlayerIds,
    [property: JsonPropertyName("gameids")] string GameIds,
    int Available,
    [property: JsonPropertyName("validTo")] string ValidTo,
    [property: JsonPropertyName("validFrom")] string ValidFrom,
    [property: JsonPropertyName("betlevel")] string BetLevel,
    string Currency,
    [property: DefaultValue("addFreeRounds")] string Method = "addFreeRounds");