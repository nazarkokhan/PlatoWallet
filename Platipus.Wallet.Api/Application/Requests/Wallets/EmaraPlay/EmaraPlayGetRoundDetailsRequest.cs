using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayGetRoundDetailsRequest(
    string? Environment, string Bet, 
    string? User = null, string? Game = null,
    string? Operator = null, string? Currency = null,
    string? Token = null) : 
        IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayGetRoundDetailsRequest, IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
    {
        private readonly IEmaraPlayGameApiClient _apiClient;
        private readonly IWalletService _walletService;

        public Handler(
            IEmaraPlayGameApiClient apiClient, 
            IWalletService walletService)
        {
            _apiClient = apiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>> Handle(
            EmaraPlayGetRoundDetailsRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _apiClient.GetRoundDetailsAsync(
                walletResponse.Data.BaseUrl, request, cancellationToken);
            
            if(clientResponse.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayGetRoundDetailsResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var response = new EmaraPlayGetRoundDetailsResponse(
                0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }

    public string? Provider { get; }
}