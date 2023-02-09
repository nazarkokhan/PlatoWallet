namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Results.Everymatrix.WithData;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using static Results.Everymatrix.EverymatrixResultFactory;

public record EverymatrixBetRequest(
        Guid Token,
        decimal Amount,
        string Currency,
        string GameId,
        string RoundId,
        string ExternalId,
        string Hash,
        EverymatrixJackpotContribution? JackpotContribution)
    : IEveryMatrixRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
{
    public class Handler : IRequestHandler<EverymatrixBetRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixBetRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
                .Select(u => new { u.UserName })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound);

            var walletRequest = request.Map(
                r => new BetRequest(
                    r.Token,
                    user.UserName,
                    r.Currency,
                    r.RoundId,
                    r.ExternalId,
                    false,
                    r.Amount));

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();

            var response = walletResult.Data.Map(d => new EverymatrixBalanceResponse(d.Balance, d.Currency));

            return Success(response);
        }
    }
}