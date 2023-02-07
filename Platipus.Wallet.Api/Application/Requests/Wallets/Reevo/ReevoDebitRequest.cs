namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Reevo;
using Results.Reevo.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using static Results.Reevo.ReevoResultFactory;

public record ReevoDebitRequest(
        string CallerId,
        string CallerPassword,
        string Action,
        int? RemoteId,
        string Username,
        string SessionId,
        double Amount,
        string GameIdHash,
        string TransactionId,
        string RoundId,
        int GameplayFinal,
        int IsFreeRoundBet,
        string? FreeRoundId,
        double? Fee,
        double? JackpotContributionInAmount,
        Guid GameSessionId,
        string Key)
    : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoDebitRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoDebitRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.GameSessionId))
                .Select(u => new { Currency = u.Currency.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Failure<ReevoSuccessResponse>(ReevoErrorCode.BetRefused);

            var walletRequest = request.Map(
                r => new BetRequest(
                    r.GameSessionId,
                    r.Username,
                    user.Currency,
                    r.RoundId,
                    r.TransactionId,
                    r.GameplayFinal is 1,
                    (decimal)r.Amount));

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();

            var response = walletResult.Data.Map(d => new ReevoSuccessResponse(d.Balance));

            return Success(response);
        }
    }
}