using System.ComponentModel.DataAnnotations;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformBetRequest(
    string Token, [MaxLength(150)] string RoundId, int Amount,
    [MaxLength(150)] string TransactionId, string Currency,
    string? BonusInstanceId = null) : 
        IRequest<IAtlasPlatformResult<AtlasPlatformCommonResponse>>
{
    public sealed class Handler :
        IRequestHandler<AtlasPlatformBetRequest, IAtlasPlatformResult<AtlasPlatformCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasPlatformResult<AtlasPlatformCommonResponse>> Handle(
            AtlasPlatformBetRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                amount: request.Amount,
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                switch (walletResult.Error)
                {
                    case ErrorCode.UserNotFound:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.SessionValidationFailed);
                    case ErrorCode.ValidationError:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request), "Unknown error has occurred");
                }
            }

            var response = new AtlasPlatformCommonResponse(
                walletResult.Data.Currency, (int)walletResult.Data.Balance, 
                walletResult.Data.UserId.ToString());

            return AtlasPlatformResultFactory.Success(response);
        }
    }
}