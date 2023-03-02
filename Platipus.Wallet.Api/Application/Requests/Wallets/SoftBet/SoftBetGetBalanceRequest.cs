namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record SoftBetGetBalanceRequest(
    string SessionId,
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
            var walletResult = await _wallet.GetBalanceAsync(
                request.SessionId,
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