namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using Base;
using Helpers;
using Responses.Vegangster;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Services.Wallet;

public sealed record VegangsterBetRequest(
        string Token,
        string PlayerId,
        string GameCode,
        string TransactionId,
        string RoundId,
        string Currency,
        int Amount)
    : IVegangsterTransactionRequest, IRequest<IVegangsterResult<VegangsterTransactionResponse>>
{
    public class Handler : IRequestHandler<VegangsterBetRequest, IVegangsterResult<VegangsterTransactionResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IVegangsterResult<VegangsterTransactionResponse>> Handle(
            VegangsterBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToVegangsterFailureResult<VegangsterTransactionResponse>();

            var data = walletResult.Data;

            var response = new VegangsterTransactionResponse(
                VegangsterResponseStatus.OK.ToString(),
                data.Currency,
                (int)MoneyHelper.ConvertToCents(data.Balance),
                data.Transaction.Id);

            return walletResult.ToVegangsterResult(response);
        }
    }
}