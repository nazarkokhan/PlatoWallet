namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Betflag;
using Results.Betflag.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using static Results.Betflag.BetflagResultFactory;

public record BetflagWinRequest(
    string Key,
    string TransactionId,
    string RoundId,
    bool RoundCompleted,
    double Win,
    double WinJp,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBetWinCancelRequest
{
    public class Handler : IRequestHandler<BetflagWinRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagWinRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == new Guid(request.Key)))
                .Select(
                    u => new
                    {
                        u.UserName,
                        Currency = u.Currency.Name
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.SessionExpired);

            var walletRequest = request.Map(
                r => new WinRequest(
                    new Guid(r.Key),
                    user.UserName,
                    user.Currency,
                    string.Empty,
                    r.RoundId,
                    r.TransactionId,
                    r.RoundCompleted,
                    (decimal)r.Win));

            var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToBetflagResult<BetflagBetWinCancelResponse>();

            var response = walletResult.Data.Map(
                d => new BetflagBetWinCancelResponse(
                    (double)d.Balance,
                    d.Currency));

            return Success(response);
        }
    }
}