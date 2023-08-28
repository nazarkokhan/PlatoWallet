namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

using System.ComponentModel;
using System.Text.Json.Serialization;

public record ReevoAddFreeRoundsGameApiRequest(
    [property: DefaultValue("reevo01")] string ApiLogin,
    [property: DefaultValue("reevopass01")] string ApiPassword,
    [property: DefaultValue("title")] string Title,
    [property: JsonPropertyName("playerids"), DefaultValue("reevo_local")] string PlayerIds,
    [property: JsonPropertyName("gameids"), DefaultValue("568,554")] string GameIds,
    [property: DefaultValue(10)] int Available,
    [property: JsonPropertyName("validTo"), DefaultValue("2023-03-25")] string ValidTo,
    [property: JsonPropertyName("validFrom"), DefaultValue("2023-03-20")] string ValidFrom,
    [property: JsonPropertyName("betlevel"), DefaultValue("min")] string BetLevel,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("reevo_platipus")] string Operator,
    [property: DefaultValue("addFreeRounds")] string Method = "addFreeRounds");