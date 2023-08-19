namespace Platipus.Wallet.Api.Application.Requests.External.Nemesis;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.NemesisGamesApi;
using Services.NemesisGamesApi.Requests;

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
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
               .Where(u => u.Username == apiRequest.UserId)
               .Include(
                    u => u.Awards
                       .Where(a => a.Id == apiRequest.BonusCode))
               .Include(u => u.Currency)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);
            if (user.Awards.Any(a => a.Id == apiRequest.BonusCode))
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            var award = new Award(
                apiRequest.BonusCode,
                DateTimeOffset.FromUnixTimeSeconds(apiRequest.ExpirationTimestamp).DateTime);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.CreateAwardAsync(
                environment.BaseUrl,
                apiRequest,
                user.Casino.SignatureKey,
                cancellationToken);

            return response;
        }
    }
}