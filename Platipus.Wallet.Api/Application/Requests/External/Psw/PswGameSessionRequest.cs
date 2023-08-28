namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.PswGameApi;
using Services.PswGameApi.Requests;

public record PswGameSessionRequest(
    [property: DefaultValue("test")] string Environment,
    LaunchMode LaunchModeType,
    bool IsBetflag,
    PswGameSessionGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<PswGameSessionRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IPswGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IPswGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            PswGameSessionRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var session = await _context.Set<Session>()
               .Where(e => e.Id == apiRequest.SessionId)
               .FirstOrDefaultAsync(cancellationToken);

            if (session is not null)
                return ResultFactory.Failure(ErrorCode.SessionAlreadyExists);

            var user = await _context.Set<User>()
               .Where(e => e.Username == apiRequest.User)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            session = new Session
            {
                User = user,
                IsTemporaryToken = true,
                Id = apiRequest.SessionId,
            };
            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.GameSessionAsync(
                environment.BaseUrl,
                apiRequest,
                request.LaunchModeType,
                request.IsBetflag,
                cancellationToken);

            return response;
        }
    }
}