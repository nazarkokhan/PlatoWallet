namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi.Requests;

public sealed record AtlasRegisterFreeSpinBonusGameApiRequest(
    string GameId,
    string BonusId,
    string CasinoId,
    int SpinsCount,
    List<Dictionary<string, int>> BetValues,
    string StartDate,
    string ExpirationDate);