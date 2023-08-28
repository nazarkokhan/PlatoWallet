namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.ComponentModel;
using System.Text.Json.Serialization;

public record ReevoRemoveFreeRoundsGameApiRequest(
    [property: DefaultValue("reevo01")] string ApiLogin,
    [property: DefaultValue("reevopass01")] string ApiPassword,
    [property: JsonPropertyName("playerids"), DefaultValue("reevo_platipus,reevo_local")] string PlayerIds,
    [property: DefaultValue("257f79216fa349ffb76c69e8577efdf8")] string FreeroundId,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("removeFreeRounds")] string Method = "removeFreeRounds");