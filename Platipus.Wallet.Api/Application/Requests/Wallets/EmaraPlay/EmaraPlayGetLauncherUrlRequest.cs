using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;

//TODO put using statements under namespace. namespace should be 1 line of code. You can change it in Rider settings
namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

//TODO Put external endpoints inside Requests/External/EmaraPlay
public sealed record EmaraPlayGetLauncherUrlRequest(
    string Environment,
    string Operator,
    string? Token,
    string Game,
    string Mode,
    string Lang,
    string Channel,
    string Jurisdiction,
    string Currency,
    string Ip,
    string? User,
    string? Lobby = null,
    string? Cashier = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
{
    public sealed class Handler
        : IRequestHandler<EmaraPlayGetLauncherUrlRequest, IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
    {
        private readonly IEmaraPlayGameApiClient _gameApiClient;
        private readonly IWalletService _walletService;

        public Handler(
            IEmaraPlayGameApiClient gameApiClient,
            IWalletService walletService)
        {
            _gameApiClient = gameApiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>> Handle(
            EmaraPlayGetLauncherUrlRequest urlRequest,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(urlRequest.Environment, cancellationToken);

            //TODO so you get walletResponse.Data.BaseUrl and what is going to happen when result is failed? Add IsFailed check
            var clientResponse = await _gameApiClient.GetLauncherUrlAsync(
                walletResponse.Data.BaseUrl,
                urlRequest,
                cancellationToken);

            // TODO again magic strings and numbers. Anyway this is external call and there is no need to follow wallet methods contract
            var response = new EmaraPlayGetLauncherUrlResponse(0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}