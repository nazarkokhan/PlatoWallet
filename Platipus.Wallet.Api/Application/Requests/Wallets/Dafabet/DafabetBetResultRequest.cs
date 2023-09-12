namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using System.Globalization;
using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record DafabetBetResultRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    bool EndRound,
    string? Device,
    string Hash) : IDafabetRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetBetResultRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetBetResultRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.PlayerId,
                request.RoundId,
                request.TransactionId,
                request.Amount,
                request.EndRound,
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
    {
        return PlayerId + Amount.ToString(CultureInfo.InvariantCulture) + GameCode + RoundId + TransactionId + EndRound.ToString().ToLower();
    }
}