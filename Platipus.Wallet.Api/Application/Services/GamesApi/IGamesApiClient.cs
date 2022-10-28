namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using DTOs.Responses;
using Results.Common.Result.WithData;

public interface IGamesApiClient
{
    Task<IResult<GetLaunchUrlResponseDto>> GetGameLinkAsync(
        string casinoId,
        Guid sessionId,
        string user,
        string currency,
        string game,
        string locale = "en",
        string lobby = "",
        string launchMode = "url",
        CancellationToken cancellationToken = default);

    Task<IResult<GetCasinoGamesListResponseDto>> GetCasinoGamesAsync(
        string casinoId,
        CancellationToken cancellationToken = default);

    Task<IResult<CreateFreebetAwardResponseDto>> CreateFreebetAwardAsync(
        string casinoId,
        string user,
        string awardId,
        string currency,
        string[] games,
        DateTime validUntil,
        int count,
        CancellationToken cancellationToken = default);
}