namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

using System.Text.Json.Serialization;
using Models;

public sealed record UranusAvailableGamesData([property: JsonPropertyName("docs")] List<UranusGameModel> Docs);