namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using TODO.EmaraPlay.Base;

public record EmaraPlayRefundRequest(
    string User,
    string Transaction,
    string OriginalTransaction,
    string Amount,
    string BonusAmount,
    string Provider,
    string Bet,
    string Game,
    string Token) : IRequest<IEmaraPlayResult<EmaraPlayResponse>>, IEmaraPlayBaseRequest
{
    public class Handler : IRequestHandler<EmaraPlayRefundRequest, IEmaraPlayResult<EmaraPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IEmaraPlayResult<EmaraPlayResponse>> Handle(EmaraPlayRefundRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Token,
                request.OriginalTransaction,
                request.Bet);

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