namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Extensions;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftBetCancelRequest(
    Guid SessionId,
    string UserName,
    string ProviderGameId,
    string RoundId,
    string TransactionId) : IRequest<ISoftBetResult<SoftBetBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftBetCancelRequest, ISoftBetResult<SoftBetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftBetResult<SoftBetBalanceResponse>> Handle(
            SoftBetCancelRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new RollbackRequest(
                    r.SessionId,
                    r.UserName,
                    r.ProviderGameId,
                    r.RoundId,
                    r.TransactionId));

            var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
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