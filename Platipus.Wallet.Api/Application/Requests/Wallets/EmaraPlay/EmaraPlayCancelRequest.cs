using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

//TODO separate out controller contract from contract that is accepted by game server.
public sealed record EmaraPlayCancelRequest(
    string? Environment, //TODO cant be null
    string Ref, //TODO the rest of parameters in separate record, this record should exactly replicate game server contract
    string? Operator = null, //TODO there is no point to use optional parameters as it is deserialized from json
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

            await _apiClient.CancelAsync(walletResponse.Data.BaseUrl, request, cancellationToken);

            var finalResponse = new EmaraPlayCancelResponse(0, "Success");
            return EmaraPlayResultFactory.Success(finalResponse);
        }
    }
}