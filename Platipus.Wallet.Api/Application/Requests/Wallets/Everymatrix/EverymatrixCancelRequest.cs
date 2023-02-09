namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record EverymatrixCancelRequest(
    Guid Token,
    string ExternalId,
    string CanceledExternalId,
    string Hash) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
{
    public class Handler : IRequestHandler<EverymatrixCancelRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixCancelRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
                .Select(u => new { u.UserName })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound);

            var transaction = await _context.Set<Transaction>()
                .Where(t => t.Id == request.ExternalId)
                .Select(
                    u => new
                    {
                        u.RoundId,
                        u.Amount
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
                return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TransactionNotFound);

            var walletRequest = request.Map(
                r => new RollbackRequest(
                    r.Token,
                    user.UserName,
                    string.Empty,
                    transaction.RoundId,
                    r.ExternalId));

            var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();

            var response = walletResult.Data.Map(d => new EverymatrixBalanceResponse(d.Balance, d.Currency));

            return EverymatrixResultFactory.Success(response);
        }
    }
}