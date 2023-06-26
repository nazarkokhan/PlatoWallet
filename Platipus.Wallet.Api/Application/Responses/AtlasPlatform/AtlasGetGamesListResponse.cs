namespace Platipus.Wallet.Api.Application.Responses.AtlasPlatform;

using Requests.Wallets.Atlas.Models;

public sealed record AtlasGetGamesListResponse(
    List<AtlasGameModel> GameList);