namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.PswGamesApi;
using Services.PswGamesApi.Requests;

public record PswFreebetAwardRequest(
    [property: DefaultValue("test")] string Environment,
    bool IsBetflag,
    PswFreebetAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<PswFreebetAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IPswGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IPswGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            PswFreebetAwardRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
               .Where(u => u.Username == apiRequest.User)
               .Include(
                    u => u.Awards
                       .Where(a => a.Id == apiRequest.AwardId))
               .Include(u => u.Currency)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            if (user.Awards.Any(a => a.Id == apiRequest.AwardId))
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            var award = new Award(apiRequest.AwardId, apiRequest.ValidUntil);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.FreebetAwardAsync(
                environment.BaseUrl,
                apiRequest,
                request.IsBetflag,
                cancellationToken);

            return response;
        }
    }
}