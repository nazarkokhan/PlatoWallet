namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using Newtonsoft.Json;

public sealed record EvenbetFailureResponse(
    [property: JsonProperty("error")] EvenbetErrorResponse Error);