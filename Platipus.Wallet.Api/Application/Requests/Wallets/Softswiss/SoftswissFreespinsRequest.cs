namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Requests.Base;
using Results.ResultToResultMappers;
using Services.Wallet;
using StartupSettings.Options;

public record SoftswissFreespinsRequest(
        string IssueId,
        string Status,
        long TotalAmount)
    : IRequest<ISoftswissResult<SoftswissBalanceResponse>>, IBaseWalletRequest
{
    public class Handler : IRequestHandler<SoftswissFreespinsRequest, ISoftswissResult<SoftswissBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;

        public Handler(IWalletService wallet, WalletDbContext context, IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
        {
            _wallet = wallet;
            _context = context;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<ISoftswissResult<SoftswissBalanceResponse>> Handle(
            SoftswissFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var award = await _context.Set<Award>()
                .Where(a => a.Id == request.IssueId)
                .Select(
                    a => new
                    {
                        a.ValidUntil,
                        a.Currency,
                        UserSession = a.User.Sessions
                            .OrderByDescending(x => x.CreatedDate)
                            .Select(s => new { s.Id })
                            .FirstOrDefault()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (award?.UserSession is null)
                return SoftswissResultFactory.Failure<SoftswissBalanceResponse>(SoftswissErrorCode.InvalidFreeSpinsIssue);

            var walletResult = await _wallet.AwardAsync(
                award.UserSession.Id,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                //TODO remove usd hardcode
                _currencyMultipliers.GetSumIn(award.Currency ?? "USD", request.TotalAmount),
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