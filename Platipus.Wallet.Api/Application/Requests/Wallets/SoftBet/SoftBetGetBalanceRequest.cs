namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Extensions;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftBetGetBalanceRequest(
    Guid SessionId,
    string UserName) : IRequest<ISoftBetResult<SoftBetBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftBetGetBalanceRequest, ISoftBetResult<SoftBetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftBetResult<SoftBetBalanceResponse>> Handle(
            SoftBetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new GetBalanceRequest(
                    r.SessionId,
                    r.UserName));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
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