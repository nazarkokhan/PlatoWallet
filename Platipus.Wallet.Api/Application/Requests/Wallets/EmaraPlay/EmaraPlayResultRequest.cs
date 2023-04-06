namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using TODO.EmaraPlay.Base;

public record EmaraPlayResultRequest(
    string User,
    string Game,
    string Bet,
    string Amount,
    string Transaction,
    string Provider,
    string Token,
    string CloseRound,
    string Jackpots,
    string? BetBonusAmount) : IRequest<IEmaraPlayResult<EmaraPlayResponse>>, IEmaraPlayBaseRequest
{
    public class Handler : IRequestHandler<EmaraPlayResultRequest, IEmaraPlayResult<EmaraPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IEmaraPlayResult<EmaraPlayResponse>> Handle(EmaraPlayResultRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.Token,
                request.Bet,
                request.Transaction,
                decimal.Parse(request.Amount));

            if (walletResult.IsFailure)
                return walletResult.ToEmaraplayResult<EmaraPlayResponse>();
            var data = walletResult.Data;

            var response = new EmaraPlayResponse(new EmaraplayBaseResponse(
                data.Currency,
                data.Balance.ToString(CultureInfo.InvariantCulture),
                data.Transaction.Id,
                data.Transaction.InternalId));

            return EmaraPlayResultFactory.Success(response);
        }
    }
}