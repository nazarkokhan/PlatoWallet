namespace Platipus.Wallet.Api.Application.Requests.External.Nemesis;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Platipus.Wallet.Api.Application.Services.PswGamesApi;
using Platipus.Wallet.Api.Application.Services.PswGamesApi.Requests;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;
using Psw;
using Services.NemesisGamesApi;
using Services.NemesisGamesApi.Requests;

public record NemesisLauncherRequest(
    [property: DefaultValue("test")] string Environment,
    [property: JsonIgnore] NemesisLauncherGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<NemesisLauncherRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly INemesisGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, INemesisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            NemesisLauncherRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var session = await _context.Set<Session>()
               .Where(e => e.Id == apiRequest.SessionToken)
               .FirstOrDefaultAsync(cancellationToken);
            if (session is not null)
                return ResultFactory.Failure(ErrorCode.SessionAlreadyExists);

            var user = await _context.Set<User>()
               .Where(e => e.Username == apiRequest.UserId)
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            session = new Session
            {
                User = user,
                IsTemporaryToken = true,
                Id = apiRequest.SessionToken,
            };
            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.LauncherAsync(
                environment.BaseUrl,
                apiRequest,
                user.Casino.SignatureKey,
                cancellationToken);

            return response;
        }
    }
}