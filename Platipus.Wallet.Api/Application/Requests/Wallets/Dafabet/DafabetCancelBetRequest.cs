namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record DafabetCancelBetRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string OriginalTransactionId,
    string Hash) : IDafabetRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetCancelBetRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetCancelBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.PlayerId,
                request.OriginalTransactionId,
                request.RoundId,
                searchByUsername: true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToDafabetResult<DafabetBalanceResponse>();
            var data = walletResult.Data;

            var response = new DafabetBalanceResponse(data.Username, data.Currency, data.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource()
        => PlayerId + Amount + GameCode + RoundId + OriginalTransactionId;
}