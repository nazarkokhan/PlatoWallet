namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base.Response;
using Microsoft.Extensions.Options;
using Services.Wallet;
using StartupSettings.Options;

public record SoftswissFreespinsRequest(
        string SessionId,
        string IssueId,
        string Status,
        long TotalAmount)
    : IRequest<ISoftswissResult<SoftswissBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftswissFreespinsRequest, ISoftswissResult<SoftswissBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;

        public Handler(IWalletService wallet, IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
        {
            _wallet = wallet;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public Task<ISoftswissResult<SoftswissBalanceResponse>> Handle(
            SoftswissFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            // var walletRequest = request.Map(r => new AwardRequest(r.SessionId, r.Game));
            //
            // var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            // if (walletResult.IsFailure)
            //     walletResult.ToSoftswissResult();
            //
            // var response = walletResult.Data.Map(
            //     d => new SoftswissBalanceResponse(_currencyMultipliers.GetSumOut(request.Currency, d.Balance)));

            return Task.FromResult<ISoftswissResult<SoftswissBalanceResponse>>(SoftswissResultFactory.Success(new SoftswissBalanceResponse(-9999)));
        }
    }
}