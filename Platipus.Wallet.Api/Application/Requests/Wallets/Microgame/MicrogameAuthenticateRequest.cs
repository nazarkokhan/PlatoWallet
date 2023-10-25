namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using System.Text.Json.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Responses.Microgame;
using Results.Microgame;
using Results.Microgame.WithData;

public sealed record MicrogameAuthenticateRequest(
    string GameId,
    string AccessToken,
    [property: JsonPropertyName("sessionId")] string SessionId,
    [property: JsonPropertyName("clientIp")] string? ClientIp,
    [property: JsonPropertyName("language")] string? Language) : IRequest<IMicrogameResult<MicrogameAuthenticateResponse>>,
                                                                 IMicrogameBaseRequest
{
    public sealed class Handler : IRequestHandler<MicrogameAuthenticateRequest, IMicrogameResult<MicrogameAuthenticateResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => _walletDbContext = walletDbContext;

        public async Task<IMicrogameResult<MicrogameAuthenticateResponse>> Handle(
            MicrogameAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            var sessionInfo = await _walletDbContext.Set<Session>()
               .Include(u => u.User)
               .Where(x => x.Id == request.SessionId)
               .Select(
                    x => new
                    {
                        x.User.Username,
                        x.User.CurrencyId,
                        x.UserId
                    })
               .FirstOrDefaultAsync(cancellationToken);

            if (sessionInfo is null)
            {
                return MicrogameResultFactory.Failure<MicrogameAuthenticateResponse>(MicrogameStatusCode.UNAVAILABLE);
            }

            var newSession = new Session
            {
                UserId = sessionInfo.UserId,
                IsTemporaryToken = true
            };

            await _walletDbContext.Set<Session>().AddAsync(newSession, cancellationToken);

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            var response = new MicrogameAuthenticateResponse(sessionInfo.Username, sessionInfo.CurrencyId, newSession.Id);
            return MicrogameResultFactory.Success(response);
        }
    }
}