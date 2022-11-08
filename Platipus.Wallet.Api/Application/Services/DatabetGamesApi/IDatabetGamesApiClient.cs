namespace Platipus.Wallet.Api.Application.Services.DatabetGamesApi;

using GamesApi.DTOs.Responses;
using Results.Psw.WithData;

public interface IDatabetGamesApiClient
{
    Task<IResult<GetDatabetLaunchUrlResponseDto>> DatabetLaunchGameAsync(
        string gameCode,
        string playerId,
        Guid playerToken,
        string currency,
        string? language,
        string hash,
        CancellationToken cancellationToken = default);
}