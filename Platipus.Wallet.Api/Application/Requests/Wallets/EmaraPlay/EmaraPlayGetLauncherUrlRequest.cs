using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayGetLauncherUrlRequest(
    string Environment,
    string Operator, string? Token, string Game, 
    string Mode, string Lang, string Channel,
    string Jurisdiction, string Currency, string Ip,
    string? User, string? Lobby = null, 
    string? Cashier = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
{
    public sealed class Handler : 
        IRequestHandler<EmaraPlayGetLauncherUrlRequest, IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
    {
        private readonly IEmaraPlayGameApiClient _gameApiClient;
        private readonly IWalletService _walletService;

        public Handler(
            IEmaraPlayGameApiClient gameApiClient, IWalletService walletService)
        {
            _gameApiClient = gameApiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>> Handle(
            EmaraPlayGetLauncherUrlRequest urlRequest, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                urlRequest.Environment, cancellationToken);
            var clientResponse = await _gameApiClient.GetLauncherUrlAsync(
                walletResponse.Data.BaseUrl, urlRequest, cancellationToken);
            if (clientResponse.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayGetLauncherUrlResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var response = new EmaraPlayGetLauncherUrlResponse(
                0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}