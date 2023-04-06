// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.EmaraPlay;

using System.Globalization;
using Base;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record EmaraPlayBetRequest(
    string User,
    string Game,
    string Bet,
    string Provider,
    string Token,
    string Transaction,
    string Amount) : IRequest<IEmaraPlayResult<EmaraPlayResponse>>, IEmaraPlayBaseRequest
{
    public class Handler : IRequestHandler<EmaraPlayBetRequest, IEmaraPlayResult<EmaraPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IEmaraPlayResult<EmaraPlayResponse>> Handle(EmaraPlayBetRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
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