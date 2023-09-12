namespace Platipus.Wallet.Api.Application.Requests.External.Parimatch;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Services.ParimatchGameApi;
using Services.ParimatchGameApi.Requests;

[PublicAPI]
public record ParimatchLauncherRequest(
    string Environment,
    string Username,
    ParimatchLauncherGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<ParimatchLauncherRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IParimatchGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IParimatchGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            ParimatchLauncherRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;
            try
            {
                var session = await _context.Set<Session>()
                   .Where(e => e.Id == apiRequest.SessionToken)
                   .FirstOrDefaultAsync(cancellationToken);
                if (session is not null)
                    return ResultFactory.Failure(ErrorCode.SessionAlreadyExists);

                var user = await _context.Set<User>()
                   .Where(e => e.Username == request.Username)
                   .Include(u => u.Casino)
                   .FirstOrDefaultAsync(cancellationToken);
                if (user is null)
                    return ResultFactory.Failure(ErrorCode.UserNotFound);

                session = new Session
                {
                    Id = apiRequest.SessionToken,
                    User = user,
                    IsTemporaryToken = true
                };
                _context.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                var response = await _gameApiClient.LauncherAsync(
                    environment.BaseUrl,
                    apiRequest,
                    cancellationToken);

                if (response is not { IsSuccess: true, Data.IsSuccess: true })
                {
                    _context.Remove(session);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return response;
            }
            catch (Exception e)
            {
                return ResultFactory.Failure(ErrorCode.Unknown, e);
            }
        }
    }
}