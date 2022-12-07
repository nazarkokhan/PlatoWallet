namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftBetEndRequest(
    Guid SessionId,
    string UserName,
    string SessionStatus) : IRequest<ISoftBetResult<SoftBetBalanceResponse>>
{
    public class Handler : IRequestHandler<SoftBetEndRequest, ISoftBetResult<SoftBetBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<ISoftBetResult<SoftBetBalanceResponse>> Handle(
            SoftBetEndRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new GetBalanceRequest(
                    r.SessionId,
                    r.UserName));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToSoftBetResult();

            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return SoftBetResultFactory.Failure<SoftBetBalanceResponse>(SoftBetErrorMessage.PlayerAuthenticationFailed);

            _context.Remove(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = walletResult.Data.Map(
                d => new SoftBetBalanceResponse(
                    (int)(d.Balance * 100),
                    d.Currency));

            return SoftBetResultFactory.Success(response);
        }
    }
}