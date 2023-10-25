namespace Platipus.Wallet.Api.Application.Responses.Microgame;

using System.Text.Json.Serialization;
using Base;
using Results.Microgame;

public sealed record MicrogameCancelReserveResponse(
    [property: JsonPropertyName("externalPropertyId")] string ExternalTransactionId) : MicrogameCommonResponse(
    MicrogameStatusCode.OK);