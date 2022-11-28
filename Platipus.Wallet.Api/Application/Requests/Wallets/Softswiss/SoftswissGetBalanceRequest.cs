namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Base.Response;
using Extensions;
using Microsoft.Extensions.Options;
using Results.Hub88.WithData.Mappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using StartupSettings.Options;

public record SoftswissGetBalanceRequest(
    Guid SessionId,
    string UserId,
    string Currency,
    string Game,
    string? GameId,
    bool? Finished) : ISoftswissBaseRequest, IRequest<ISoftswissResult<SoftswissBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftswissGetBalanceRequest, ISoftswissResult<SoftswissBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;

        public Handler(IWalletService wallet, IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
        {
            _wallet = wallet;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<ISoftswissResult<SoftswissBalanceResponse>> Handle(
            SoftswissGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(r => new GetBalanceRequest(r.SessionId, r.UserId));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToSoftswissResult<SoftswissBalanceResponse>();

            var response = walletResult.Data.Map(
                d => new SoftswissBalanceResponse(_currencyMultipliers.GetSumOut(request.Currency, d.Balance)));

            return SoftswissResultFactory.Success(response);
        }
    }
}