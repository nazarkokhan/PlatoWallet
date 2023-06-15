using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformGetClientBalanceRequest(
    string Token) : 
    IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformCommonResponse>>
{
    public sealed class Handler :
        IRequestHandler<AtlasPlatformGetClientBalanceRequest,
            IAtlasPlatformResult<AtlasPlatformCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasPlatformResult<AtlasPlatformCommonResponse>> Handle(
            AtlasPlatformGetClientBalanceRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                switch (walletResult.Error)
                {
                    case ErrorCode.UserIsDisabled:
                    case ErrorCode.UserNotFound:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.SessionValidationFailed);
                    case ErrorCode.UnknownGetBalanceException:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.InternalError);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var response = new AtlasPlatformCommonResponse(
                walletResult.Data.Currency, 
                Convert.ToInt32(walletResult.Data.Balance), 
                walletResult.Data.UserId.ToString()
                );
            return AtlasPlatformResultFactory.Success(response);
        }
    }
}