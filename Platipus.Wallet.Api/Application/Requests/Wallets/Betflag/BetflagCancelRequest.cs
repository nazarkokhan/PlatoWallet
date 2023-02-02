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

public record BetflagCancelRequest(
    string Key,
    string TransactionId,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBetWinCancelRequest
{
    public class Handler : IRequestHandler<BetflagCancelRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagCancelRequest request,
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

            var transaction = await _context.Set<Transaction>()
                .Where(t => t.Id == request.TransactionId)
                .Select(
                    u => new
                    {
                        u.RoundId,
                        u.Amount
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
                return Failure<BetflagBetWinCancelResponse>(BetflagErrorCode.CancelReferBetNotExists);

            var walletRequest = request.Map(
                r => new RollbackRequest(
                    new Guid(r.Key),
                    user.UserName,
                    string.Empty,
                    transaction.RoundId,
                    r.TransactionId));

            var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
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