using System.ComponentModel.DataAnnotations;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformWinRequest(
    string Token, string ClientId,
    [MaxLength(150)] string RoundId, int Amount,
    [MaxLength(150)] string TransactionId) : 
        IRequest<IAtlasPlatformResult<AtlasPlatformCommonResponse>>, IAtlasPlatformRequest
{
    public sealed class Handler : 
        IRequestHandler<AtlasPlatformWinRequest, IAtlasPlatformResult<AtlasPlatformCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasPlatformResult<AtlasPlatformCommonResponse>> Handle(
            AtlasPlatformWinRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                request.Amount, 
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                switch (walletResult.Error)
                {
                    case ErrorCode.UnknownWinException:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.InternalError);
                    case ErrorCode.InvalidCurrency:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.CurrencyMismatchException); 
                    case ErrorCode.RoundAlreadyFinished:
                    case ErrorCode.RoundAlreadyExists:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.GameRoundNotPreviouslyCreated);
                    case ErrorCode.UserIsDisabled:
                    case ErrorCode.UserNotFound:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.SessionValidationFailed);
                    case ErrorCode.TransactionAlreadyExists:
                        return AtlasPlatformResultFactory.Failure<AtlasPlatformCommonResponse>(
                            AtlasPlatformErrorCode.TransactionAlreadyProcessed);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request), "Unknown error has occurred");
                }
            }

            var response = new AtlasPlatformCommonResponse(
                walletResult.Data.Currency, (int)walletResult.Data.Balance, 
                walletResult.Data.UserId.ToString()
                );
            return AtlasPlatformResultFactory.Success(response);
        }
    }
}