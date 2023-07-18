namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using Newtonsoft.Json;
using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;

public sealed record EvenbetGetGamesResponse([property: JsonProperty("games")] List<EvenbetGameModel> Games);