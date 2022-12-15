namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Extensions;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftBetBetRequest(
    Guid SessionId,
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
            var walletRequest = request.Map(
                r => new BetRequest(
                    r.SessionId,
                    r.UserName,
                    r.Currency,
                    r.RoundId,
                    r.TransactionId,
                    false,
                    r.Amount / 100));

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToSoftBetResult();

            var response = walletResult.Data.Map(
                d => new SoftBetBalanceResponse(
                    (int)(d.Balance * 100),
                    d.Currency));

            return SoftBetResultFactory.Success(response);
        }
    }
}