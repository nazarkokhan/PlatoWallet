namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using Newtonsoft.Json;

public abstract record EvenbetCommonResponse(
    [property: JsonProperty("balance")] int Balance,
    [property: JsonProperty("timestamp")] string Timestamp);