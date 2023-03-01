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

public record ReevoRollbackRequest(
    string CallerId,
    string CallerPassword,
    string Action,
    int? RemoteId,
    string Username,
    string SessionId,
    double Amount,
    string? GameIdHash,
    string TransactionId,
    string RoundId,
    int GameplayFinal,
    Guid GameSessionId,
    string Key) : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoRollbackRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.GameSessionId))
                .Select(u => new { Currency = u.Currency.Name })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Failure<ReevoSuccessResponse>(ReevoErrorCode.GeneralError);

            var walletRequest = request.Map(
                r => new RollbackRequest(
                    r.GameSessionId,
                    r.Username,
                    string.Empty,
                    r.RoundId,
                    r.TransactionId));

            var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();

            var response = walletResult.Data.Map(d => new ReevoSuccessResponse(d.Balance));

            return Success(response);
        }
    }
}