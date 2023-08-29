namespace Platipus.Wallet.Api.Application.Services.AtlasGameApi.Requests;

public sealed record AtlasGameLaunchGameApiRequest(
    string GameId,
    bool Demo,
    bool IsMobile,
    string Token,
    string CasinoId,
    string Language,
    string CashierUrl,
    string LobbyUrl);