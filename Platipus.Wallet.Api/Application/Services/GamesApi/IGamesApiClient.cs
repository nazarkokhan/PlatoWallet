namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using Domain.Entities.Enums;
using DTOs.Responses;

public interface IGamesApiClient
{
    Task<IPswResult<GetLaunchUrlResponseDto>> GetLaunchUrlAsync(
        CasinoProvider casinoProvider,
        string casinoId,
        Guid sessionId,
        string user,
        string currency,
        string game,
        string locale = "en",
        string lobby = "",
        string launchMode = "url",
        CancellationToken cancellationToken = default);

    Task<IPswResult<PswGetCasinoGamesListGamesApiResponseDto>> GetCasinoGamesAsync(
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