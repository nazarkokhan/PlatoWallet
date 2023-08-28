namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus;

using Base;
using Data;
using Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Uranus;
using Platipus.Wallet.Api.Application.Results.Uranus.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record UranusPromoWinRequest(
        string PlayerId,
        string TransactionId,
        string SessionToken,
        string Amount,
        string Currency,
        string? Payload)
    : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
{
    public sealed class Handler
        : IRequestHandler<UranusPromoWinRequest, IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>> Handle(
            UranusPromoWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.AwardAsync(
                sessionId: request.PlayerId,
                roundId: "0",
                transactionId: request.TransactionId,
                amount: decimal.Parse(request.Amount),
                awardId: "some award id",
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToUranusFailureResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>();
            var response = new UranusSuccessResponse<UranusCommonDataWithTransaction>(
                new UranusCommonDataWithTransaction(
                    walletResult.Data?.Currency, 
                    walletResult.Data!.Balance,
                    "some transactionId"
                ));
            return UranusResultFactory.Success(response);
        }
    }
}