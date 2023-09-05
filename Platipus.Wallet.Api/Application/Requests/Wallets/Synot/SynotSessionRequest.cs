namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Responses.Synot;
using Results.Synot;
using Results.Synot.WithData;

public sealed record SynotSessionRequest(string? Token) : ISynotBaseRequest, IRequest<ISynotResult<SynotSessionResponse>>
{
    public sealed class Handler : IRequestHandler<SynotSessionRequest, ISynotResult<SynotSessionResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<ISynotResult<SynotSessionResponse>> Handle(
            SynotSessionRequest request,
            CancellationToken cancellationToken)
        {
            var sessionData = await _walletDbContext.Set<Session>()
               .TagWith("Session")
               .Include(u => u.User.Currency)
               .Where(s => s.Id == request.Token!)
               .FirstOrDefaultAsync(cancellationToken);

            if (sessionData is null)
            {
                return SynotResultFactory.Failure<SynotSessionResponse>(SynotError.INVALID_TOKEN);
            }
            
            var newToken = Guid.NewGuid().ToString();

            var newSession = new Session
            {
                Id = newToken,
                UserId = sessionData.UserId,
                IsTemporaryToken = true
            };
            await _walletDbContext.Set<Session>().AddAsync(newSession, cancellationToken);

            _walletDbContext.Remove(sessionData);
            
            await _walletDbContext.SaveChangesAsync(cancellationToken);
            
            var response = new SynotSessionResponse(
                sessionData.UserId,
                "some game id",
                sessionData.User.Currency.Id,
                newToken
                );

            return SynotResultFactory.Success(response);
        }
    }
}