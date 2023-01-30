namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Extensions;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftBetWinRequest(
    Guid SessionId,
    string UserName,
    string Currency,
    string ProviderGameId,
    decimal Amount,
    string RoundId,
    string TransactionId,
    bool CloseRound) : IRequest<ISoftBetResult<SoftBetBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftBetWinRequest, ISoftBetResult<SoftBetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftBetResult<SoftBetBalanceResponse>> Handle(
            SoftBetWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new WinRequest(
                    r.SessionId,
                    r.UserName,
                    r.Currency,
                    r.ProviderGameId,
                    r.RoundId,
                    r.TransactionId,
                    r.CloseRound,
                    r.Amount / 100));

            var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToSoftBetResult<SoftBetBalanceResponse>();

            var response = walletResult.Data.Map(
                d => new SoftBetBalanceResponse(
                    (int)(d.Balance * 100),
                    d.Currency));

            return SoftBetResultFactory.Success(response);
        }
    }
}