namespace Platipus.Wallet.Api.Application.Requests.External.Nemesis;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Services.NemesisGameApi;
using Services.NemesisGameApi.Requests;

[PublicAPI]
public record NemesisCreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    NemesisCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<NemesisCreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly INemesisGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, INemesisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            NemesisCreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);

            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
               .Where(u => u.Username == apiRequest.UserId)
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.BonusCode)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is not null)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            var now = DateTime.UtcNow;
            var maxAwardLifetime = TimeSpan.FromDays(30);

            DateTime expirationTime;
            if (apiRequest.ExpirationTimestamp is null)
            {
                expirationTime = now + maxAwardLifetime;
            }
            else
            {
                expirationTime = DateTimeOffset.FromUnixTimeSeconds(apiRequest.ExpirationTimestamp.Value)
                   .DateTime
                   .ToUniversalTime();
            }

            award = new Award(
                apiRequest.BonusCode,
                expirationTime);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.CreateAwardAsync(
                environment.BaseUrl,
                apiRequest,
                user.Casino.SignatureKey,
                cancellationToken);

            if (response is { IsSuccess: true, Data.IsSuccess: true })
                await _context.Database.CommitTransactionAsync(cancellationToken);
            else
                await _context.Database.RollbackTransactionAsync(cancellationToken);

            return response;
        }
    }
}