namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using Newtonsoft.Json;

public record BetConstructDataRequest(
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? Token,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? TransactionId,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? RoundId,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? GameId,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? CurrencyId,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] decimal? BetAmount,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? BetInfo
);
