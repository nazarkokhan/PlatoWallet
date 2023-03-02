namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record SoftBetBetRequest(
    string SessionId,
    string UserName,
    string Currency,
    string ProviderGameId,
    decimal Amount,
    string RoundId,
    string TransactionId) : IRequest<ISoftBetResult<SoftBetBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftBetBetRequest, ISoftBetResult<SoftBetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftBetResult<SoftBetBalanceResponse>> Handle(
            SoftBetBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.SessionId,
                request.RoundId,
                request.TransactionId,
                request.Amount / 100,
                request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSoftBetResult<SoftBetBalanceResponse>();
            var data = walletResult.Data;

            var response = new SoftBetBalanceResponse(
                (int)(data.Balance * 100),
                data.Currency);

            return SoftBetResultFactory.Success(response);
        }
    }
}