namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using System.Text.Json.Serialization;
using Models;

public sealed record VegangsterGetAvailableGamesResponse([property: JsonPropertyName("games")] List<VegangsterGameModel> Games);