namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using Results.Evoplay;
using Results.Evoplay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvoplayAwardRequest(
        string PlayerId,
        string TransactionId,
        string SessionToken,
        string Amount,
        string Currency,
        string? Payload)
    : IEvoplayRequest, IRequest<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
{
    public sealed class Handler
        : IRequestHandler<EvoplayAwardRequest, IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>> Handle(
            EvoplayAwardRequest request,
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
                return walletResult.ToEvoplayFailureResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>();
            var response = new EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>(
                new EvoplayCommonDataWithTransaction(
                    walletResult.Data?.Currency, 
                    walletResult.Data!.Balance,
                    "some transactionId"
                ));
            return EvoplayResultFactory.Success(response);
        }
    }
}