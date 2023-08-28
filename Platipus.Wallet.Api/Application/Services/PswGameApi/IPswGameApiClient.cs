namespace Platipus.Wallet.Api.Application.Services.PswGameApi;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using Requests;
using Responses;

public interface IPswGameApiClient
{
    Task<IResult<IHttpClientResult<PswGameSessionGameApiResponse, PswErrorGameApiResponse>>> GameSessionAsync(
        Uri baseUrl,
        PswGameSessionGameApiRequest request,
        LaunchMode launchModeType,
        bool isBetflag = false,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<PswGameListGameApiResponse, PswErrorGameApiResponse>>> GameListAsync(
        Uri baseUrl,
        PswGameListGameApiRequest request,
        bool isBetflag = false,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<PswFreebetAwardGameApiResponse, PswErrorGameApiResponse>>> FreebetAwardAsync(
        Uri baseUrl,
        PswFreebetAwardGameApiRequest request,
        bool isBetflag = false,
        CancellationToken cancellationToken = default);

    Task<IResult<IHttpClientResult<PswGameBuyGameApiResponse, PswErrorGameApiResponse>>> GameBuyAsync(
        Uri baseUrl,
        PswGameBuyGameApiRequest request,
        bool isBetflag = false,
        CancellationToken cancellationToken = default);
}