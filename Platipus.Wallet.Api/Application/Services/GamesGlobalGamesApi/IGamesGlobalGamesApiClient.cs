namespace Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;

public interface IGamesGlobalGamesApiClient
{
    Task<IResult<string?>> GetLaunchUrlAsync(
        Guid token,
        CancellationToken cancellationToken = default);
}