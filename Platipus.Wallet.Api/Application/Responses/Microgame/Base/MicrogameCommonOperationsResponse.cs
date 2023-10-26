namespace Platipus.Wallet.Api.Application.Responses.Microgame.Base;

using System.Text.Json.Serialization;
using Results.Microgame;

public abstract record MicrogameCommonOperationsResponse(
    MicrogameStatusCode StatusCode,
    [property: JsonPropertyName("externalTransactionId")] string ExternalTransactionId,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("sessionBalance")] decimal SessionBalance) : MicrogameCommonResponse(StatusCode);