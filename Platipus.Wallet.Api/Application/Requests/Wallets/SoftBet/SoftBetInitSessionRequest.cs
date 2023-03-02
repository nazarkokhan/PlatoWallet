namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet;

using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ISoftBet;
using Results.ISoftBet.WithData;

public record SoftBetInitSessionRequest(
    string Token,
    string UserName) : IRequest<ISoftBetResult<SoftBetInitSessionRequest.Response>>
{
    public class Handler : IRequestHandler<SoftBetInitSessionRequest, ISoftBetResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<ISoftBetResult<Response>> Handle(
            SoftBetInitSessionRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.Username == request.UserName)
                .Select(
                    u => new
                    {
                        u.Id,
                        UserName = u.Username,
                        u.Balance,
                        CurrencyName = u.Currency.Id,
                        u.IsDisabled,
                        Casino = new
                        {
                            u.Casino.Id,
                            u.Casino.SignatureKey
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return SoftBetResultFactory.Failure<Response>(SoftBetErrorMessage.PlayerAuthenticationFailed);

            if (user.Casino.SignatureKey != request.Token)
                return SoftBetResultFactory.Failure<Response>(SoftBetErrorMessage.PlayerAuthenticationFailed);

            var newSession = new Session
            {
                UserId = user.Id,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };
            _context.Add(newSession);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new Response(
                newSession.Id,
                request.UserName,
                request.UserName,
                user.CurrencyName,
                (int)(user.Balance * 100));

            return SoftBetResultFactory.Success(response);
        }
    }

    public record Response(
        string SessionId,
        string PlayerId,
        string Username,
        string Currency,
        int Balance) : SoftBetBaseResponse;
}