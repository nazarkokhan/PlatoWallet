namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ISoftBet;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record SoftBetEndRequest(
    string SessionId,
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
            var walletResult = await _wallet.GetBalanceAsync(
                request.SessionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSoftBetResult<SoftBetBalanceResponse>();
            var data = walletResult.Data;

            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return SoftBetResultFactory.Failure<SoftBetBalanceResponse>(SoftBetErrorMessage.PlayerAuthenticationFailed);

            _context.Remove(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new SoftBetBalanceResponse(
                (int)(data.Balance * 100),
                data.Currency);

            return SoftBetResultFactory.Success(response);
        }
    }
}