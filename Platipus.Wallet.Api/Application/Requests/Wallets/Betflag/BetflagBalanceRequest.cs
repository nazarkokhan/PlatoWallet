namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Results.Betflag;
using Results.Betflag.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record BetflagBalanceRequest(
    string Key,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBalanceResponse>>, IBetflagRequest
{
    public class Handler : IRequestHandler<BetflagBalanceRequest, IBetflagResult<BetflagBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetflagResult<BetflagBalanceResponse>> Handle(
            BetflagBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(request.Key, cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetflagResult<BetflagBalanceResponse>();
            var data = walletResult.Data;

            var response = new BetflagBalanceResponse(
                (double)data.Balance,
                data.Currency,
                data.UserId.ToString(),
                data.Username);

            return BetflagResultFactory.Success(response);
        }
    }
}