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

public record EverymatrixWinRequest(
    Guid Token,
    decimal Amount,
    string Currency,
    string BonusId,
    int GameId,
    string RoundId,
    string ExternalId,
    string BetExternalId,
    string Hash,
    bool? RoundEnd,
    EverymatrixJackpotPayout? JackpotPayout) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
{
    public class Handler : IRequestHandler<EverymatrixWinRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixWinRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
                .Select(u => new { u.UserName })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound);

            var walletRequest = request.Map(
                r => new WinRequest(
                    r.Token,
                    user.UserName,
                    r.Currency,
                    r.GameId.ToString(),
                    r.RoundId,
                    r.ExternalId,
                    r.RoundEnd ?? false,
                    r.Amount));

            var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();

            var response = walletResult.Data.Map(d => new EverymatrixBalanceResponse(d.Balance, d.Currency));

            return EverymatrixResultFactory.Success(response);
        }
    }
}