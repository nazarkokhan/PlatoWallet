namespace Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;

public interface IGamesGlobalGamesApiClient
{
    Task<IResult<string?>> GetLaunchUrlAsync(
        Uri baseUrl,
        Guid token,
        string game,
        CancellationToken cancellationToken = default);
}