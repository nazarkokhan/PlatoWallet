namespace Platipus.Wallet.Api.Application.Requests.External.Parimatch;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Services.ParimatchGameApi;
using Services.ParimatchGameApi.Requests;

[PublicAPI]
public record ParimatchCreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    ParimatchCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<ParimatchCreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IParimatchGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IParimatchGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            ParimatchCreateAwardRequest request,
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
               .Where(u => u.Username == apiRequest.PlayerId)
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var expirationTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(apiRequest.EndDate))
               .DateTime
               .ToUniversalTime();
            var now = DateTime.UtcNow;

            if (expirationTime < now)
                expirationTime = now + TimeSpan.FromDays(30);

            var response = await _gameApiClient.CreateAwardAsync(
                environment.BaseUrl,
                apiRequest,
                cancellationToken);

            if (response is { IsSuccess: true, Data.IsSuccess: true })
            {
                var award = new Award(
                    response.Data.Data.GiftId,
                    expirationTime);

                user.Awards.Add(award);
                _context.Update(user);

                await _context.SaveChangesAsync(cancellationToken);

                await _context.Database.CommitTransactionAsync(cancellationToken);
            }
            else
                await _context.Database.RollbackTransactionAsync(cancellationToken);

            return response;
        }
    }
}