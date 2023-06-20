using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayAwardRequest(
    string? Environment, string User, string Count, string EndDate, 
    string Currency, List<string>? Games = null, string? Code = null,
    string? MinBet = "0", string? MaxBet = null, string? StartDate = null, 
    string? Operator = null, string? Token = null, string? Provider = null) : 
    IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayAwardResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayAwardRequest, IEmaraPlayResult<EmaraPlayAwardResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly IEmaraPlayGameApiClient _apiClient;

        public Handler(
            IEmaraPlayGameApiClient apiClient, IWalletService walletService)
        {
            _apiClient = apiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayAwardResponse>> Handle(
            EmaraPlayAwardRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _apiClient.GetAwardAsync(
                walletResponse.Data.BaseUrl, request, cancellationToken);

            if (clientResponse.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayAwardResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var data = clientResponse.Data.Data.Result;
            var response = new EmaraPlayAwardResponse(
                new EmaraplayAwardResult(data.Ref));
            return EmaraPlayResultFactory.Success(response);
        }
    }
    
}