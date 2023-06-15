using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformGetClientBalanceRequest(
    string Token) : 
    IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformGetClientBalanceResponse>>
{
    public sealed class Handler :
        IRequestHandler<AtlasPlatformGetClientBalanceRequest,
            IAtlasPlatformResult<AtlasPlatformGetClientBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasPlatformResult<AtlasPlatformGetClientBalanceResponse>> Handle(
            AtlasPlatformGetClientBalanceRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return AtlasPlatformResultFactory.Failure<AtlasPlatformGetClientBalanceResponse>(
                    AtlasPlatformErrorCode.SessionValidationFailed);

            var response = new AtlasPlatformGetClientBalanceResponse(
                walletResult.Data.Currency, 
                Convert.ToInt32(walletResult.Data.Balance), 
                walletResult.Data.UserId.ToString()
                );
            return AtlasPlatformResultFactory.Success(response);
        }
    }
}