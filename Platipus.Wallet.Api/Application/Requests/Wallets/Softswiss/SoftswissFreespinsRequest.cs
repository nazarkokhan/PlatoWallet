namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base.Response;
using Microsoft.Extensions.Options;
using Results.ResultToResultMappers;
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

        public async Task<ISoftswissResult<SoftswissBalanceResponse>> Handle(
            SoftswissFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.AwardAsync(
                request.SessionId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                request.TotalAmount,
                request.IssueId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSoftswissResult<SoftswissBalanceResponse>();
            var data = walletResult.Data;

            var response = new SoftswissBalanceResponse(_currencyMultipliers.GetSumOut(data.Currency, data.Balance));

            return SoftswissResultFactory.Success(response);
        }
    }
}