using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Models;

namespace Platipus.Wallet.Api.Application.Responses.AtlasPlatform;

public sealed record AtlasPlatformGetGamesResponse(
    List<AtlasPlatformGameModel> GameList);