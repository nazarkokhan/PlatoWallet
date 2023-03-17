namespace Platipus.Wallet.Api.Application.Services.ReevoGamesApi.DTO;

using System.ComponentModel;
using System.Text.Json.Serialization;

public record ReevoRemoveFreeRoundsGameApiRequest(
    string ApiLogin,
    string ApiPassword,
    [property: JsonPropertyName("playerids")] string PlayerIds,
    string FreeroundId,
    string Currency,
    [property: DefaultValue("removeFreeRounds")] string Method = "removeFreeRounds");