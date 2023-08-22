namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi.Responses;

using System.Text.Json.Serialization;
using JetBrains.Annotations;

[PublicAPI]
public record NemesisCurrenciesGameApiResponse(
    string Iso,
    string Symbol,
    string Multiplier,
    string Converter,
    bool Crypto,
    [property: JsonPropertyName("realConverter")] double RealConverter);