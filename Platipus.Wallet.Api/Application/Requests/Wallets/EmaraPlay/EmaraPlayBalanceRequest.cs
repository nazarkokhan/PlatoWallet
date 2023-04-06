// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.EmaraPlay;

using System.Globalization;
using Base;
using Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;

public record EmaraPlayBalanceRequest(
        string User,
        string Provider,
        string Token)
    : IRequest<IEmaraPlayResult<EmaraPlayResponse>>, IEmaraPlayBaseRequest
{
    public class Handler : IRequestHandler<EmaraPlayBalanceRequest, IEmaraPlayResult<EmaraPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEmaraPlayResult<EmaraPlayResponse>> Handle(
            EmaraPlayBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEmaraplayResult<EmaraPlayResponse>();
            var data = walletResult.Data;

            var response = new EmaraPlayResponse(
                new EmaraPlayBalanceResponse(data.Balance.ToString(CultureInfo.InvariantCulture), data.Currency));

            return EmaraPlayResultFactory.Success(response);
        }
    }
}