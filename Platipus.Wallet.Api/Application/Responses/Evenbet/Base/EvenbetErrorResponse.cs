namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using Newtonsoft.Json;

public sealed record EvenbetErrorResponse(
    [property: JsonProperty("code")] int Code,
    [property: JsonProperty("message")] string Message);