namespace Platipus.Wallet.Api.Application.Services.AtlasGamesApi.Requests;

public sealed record AtlasAssignFreeSpinBonusGameApiRequest(
    string BonusId,
    string BonusInstanceId,
    string CasinoId,
    string ClientId,
    string Currency);