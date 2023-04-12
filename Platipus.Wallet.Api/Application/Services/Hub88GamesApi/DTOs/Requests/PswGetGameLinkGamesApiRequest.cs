namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record PswGetGameLinkGamesApiRequest(
    string CasinoId,
    string SessionId,
    string User,
    string Currency,
    string Game,
    string Locale,
    string Lobby,
    string LaunchMode,
    int Rci) : IPswGamesApiBaseRequest;