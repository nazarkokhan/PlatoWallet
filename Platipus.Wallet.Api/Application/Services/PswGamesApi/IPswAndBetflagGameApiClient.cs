namespace Platipus.Wallet.Api.Application.Services.PswGamesApi;

using DTOs.Responses;
using Domain.Entities.Enums;
using DTOs.Requests;

public interface IPswAndBetflagGameApiClient
{
    Task<IPswResult<GetLaunchUrlResponseDto>> GetLaunchUrlAsync(
        Uri baseUrl,
        WalletProvider walletProvider,
        string casinoId,
        string sessionId,
        string user,
        string currency,
        string game,
        LaunchMode launchModeType,
        int? rci,
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

    Task<IPswResult<PswGameBuyGamesApiResponseDto>> GameBuyAsync(
        PswGameBuyGamesApiRequest request,
        CancellationToken cancellationToken = default);
}