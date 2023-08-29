using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.ComponentModel;
using Services.EmaraPlayGameApi;
using Services.EmaraPlayGameApi.Requests;

public sealed record EmaraPlayGetLauncherUrlRequest(
    [property: DefaultValue("local")]string Environment,
    EmaraplayGetLauncherUrlGameApiRequest ApiRequest,
    string? Token) : IEmaraPlayBaseRequest, 
        IRequest<IEmaraPlayResult<EmaraPlayCommonBoxResponse<EmaraplayGetLauncherResult>>>
{
    public sealed class Handler
        : IRequestHandler<EmaraPlayGetLauncherUrlRequest, 
            IEmaraPlayResult<EmaraPlayCommonBoxResponse<EmaraplayGetLauncherResult>>>
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

        public async Task<IEmaraPlayResult<EmaraPlayCommonBoxResponse<EmaraplayGetLauncherResult>>> Handle(
            EmaraPlayGetLauncherUrlRequest urlRequest,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(urlRequest.Environment, cancellationToken);
            
            var clientResponse = await _gameApiClient.GetLauncherUrlAsync(
                walletResponse.Data.BaseUrl,
                urlRequest.ApiRequest,
                cancellationToken);
            
            if (clientResponse.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayCommonBoxResponse<EmaraplayGetLauncherResult>>(
                    EmaraPlayErrorCode.InternalServerError);

            var response = new EmaraPlayCommonBoxResponse<EmaraplayGetLauncherResult>(
                new EmaraplayGetLauncherResult(clientResponse.Data.Data.Result.Url)
                );
            return EmaraPlayResultFactory.Success(response);
        }
    }
}