namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using Base;
using Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Services.EmaraPlayGamesApi.Requests;

public sealed record EmaraPlayCancelRequest(
    string Environment,
    EmaraplayCancelGameApiRequest ApiRequest,
    string? Token = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayCancelResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayCancelRequest, IEmaraPlayResult<EmaraPlayCancelResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly IEmaraPlayGameApiClient _apiClient;

        public Handler(
            IEmaraPlayGameApiClient apiClient,
            IWalletService walletService)
        {
            _apiClient = apiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayCancelResponse>> Handle(
            EmaraPlayCancelRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            await _apiClient.CancelAsync(walletResponse.Data.BaseUrl, request.ApiRequest, cancellationToken);

            var finalResponse = new EmaraPlayCancelResponse(EmaraPlayErrorCode.Success);
            return EmaraPlayResultFactory.Success(finalResponse);
        }
    }
}