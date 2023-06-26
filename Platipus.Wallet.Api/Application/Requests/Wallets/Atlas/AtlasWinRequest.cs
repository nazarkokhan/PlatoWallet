namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using System.ComponentModel.DataAnnotations;
using Base;
using Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.Atlas;
using Results.Atlas.WithData;

public sealed record AtlasWinRequest(
    string Token, string ClientId,
    [MaxLength(150)] string RoundId, int Amount,
    [MaxLength(150)] string TransactionId) : 
        IRequest<IAtlasResult<AtlasCommonResponse>>, IAtlasRequest
{
    public sealed class Handler : 
        IRequestHandler<AtlasWinRequest, IAtlasResult<AtlasCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasResult<AtlasCommonResponse>> Handle(
            AtlasWinRequest request, CancellationToken cancellationToken)
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
                        return AtlasResultFactory.Failure<AtlasCommonResponse>(
                            AtlasErrorCode.InternalError);
                    case ErrorCode.InvalidCurrency:
                        return AtlasResultFactory.Failure<AtlasCommonResponse>(
                            AtlasErrorCode.CurrencyMismatchException); 
                    case ErrorCode.RoundAlreadyFinished:
                    case ErrorCode.RoundAlreadyExists:
                        return AtlasResultFactory.Failure<AtlasCommonResponse>(
                            AtlasErrorCode.GameRoundNotPreviouslyCreated);
                    case ErrorCode.UserIsDisabled:
                    case ErrorCode.UserNotFound:
                        return AtlasResultFactory.Failure<AtlasCommonResponse>(
                            AtlasErrorCode.SessionValidationFailed);
                    case ErrorCode.TransactionAlreadyExists:
                        return AtlasResultFactory.Failure<AtlasCommonResponse>(
                            AtlasErrorCode.TransactionAlreadyProcessed);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request), "Unknown error has occurred");
                }
            }

            var response = new AtlasCommonResponse(
                walletResult.Data.Currency, (int)walletResult.Data.Balance, 
                walletResult.Data.UserId.ToString()
                );
            return AtlasResultFactory.Success(response);
        }
    }
}