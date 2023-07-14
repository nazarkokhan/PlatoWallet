namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Services.Wallet;

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
                amount: decimal.Parse(request.Amount),
                transactionId: request.TransactionId,
                currency: request.Currency,
                roundId: "0",
                awardId: "some award id",
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
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