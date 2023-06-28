namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi.Requests;

using Application.Requests.Wallets.Atlas.Models;

public sealed record AtlasRegisterFreeSpinBonusGameApiRequest(
    string GameId,
    string BonusId,
    string CasinoId,
    int SpinsCount,
    List<AtlasBetValueModel> BetValues,
    string StartDate,
    string ExpirationDate);