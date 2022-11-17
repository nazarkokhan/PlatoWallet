namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using DTOs.Responses;
using Results.Hub88.WithData;

public interface IGamesApiClient
{
    Task<IHub88Result<GetHub88LaunchUrlResponseDto>> GetHub88GameLinkAsync(
        GetHub88GameLinkRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IPswResult<GetLaunchUrlResponseDto>> GetPswGameLinkAsync(
        string casinoId,
        Guid sessionId,
        string user,
        string currency,
        string game,
        string locale = "en",
        string lobby = "",
        string launchMode = "url",
        CancellationToken cancellationToken = default);

    Task<IPswResult<GetCasinoGamesListResponseDto>> GetCasinoGamesAsync(
        string casinoId,
        CancellationToken cancellationToken = default);

    Task<IPswResult<CreateFreebetAwardResponseDto>> CreateFreebetAwardAsync(
        string casinoId,
        string user,
        string awardId,
        string currency,
        string[] games,
        DateTime validUntil,
        int count,
        CancellationToken cancellationToken = default);
}