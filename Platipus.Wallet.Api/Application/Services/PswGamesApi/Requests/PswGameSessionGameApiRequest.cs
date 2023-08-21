namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.Requests;

public record PswGameSessionGameApiRequest(
    string CasinoId,
    string SessionId,
    string User,
    string Currency,
    string Game,
    string Locale,
    string Lobby,
    string LaunchMode,
    int Rci) : IPswGameApiBaseRequest;