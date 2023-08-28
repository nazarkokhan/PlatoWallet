namespace Platipus.Wallet.Api.Application.Services;

using Platipus.Wallet.Api.Application.Results.HttpClient.WithData;
using PswGameApi;
using PswGameApi.Requests;
using PswGameApi.Responses;

public static class GameApiClientExtensions
{
    public static async Task<IResult<IHttpClientResult<PswGameSessionGameApiResponse, PswErrorGameApiResponse>>>
        GameSessionAsync(
            this IPswGameApiClient pswGameApiClient,
            Uri baseUrl,
            string casinoId,
            string sessionId,
            string user,
            string currency,
            string game,
            LaunchMode launchModeType,
            int? rci,
            bool isBetflag = false,
            string locale = "en",
            string lobby = "",
            string launchMode = "url",
            CancellationToken cancellationToken = default)
    {
        var response = await pswGameApiClient.GameSessionAsync(
            baseUrl,
            new PswGameSessionGameApiRequest(
                casinoId,
                sessionId,
                user,
                currency,
                game,
                locale,
                lobby,
                launchMode,
                rci ?? 0),
            launchModeType,
            isBetflag,
            cancellationToken);

        return response;
    }
}