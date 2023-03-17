namespace Platipus.Wallet.Api.Application.Requests.External.Reevo;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ReevoGamesApi;
using Services.ReevoGamesApi.DTO;

public record ReevoGetGameRequest(
    string Environment,
    ReevoGetGameGameApiRequest ApiRequest) : IRequest<IResult<object>>
{
    public class Handler : IRequestHandler<ReevoGetGameRequest, IResult<object>>
    {
        private readonly IReevoGameApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(IReevoGameApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult<object>> Handle(
            ReevoGetGameRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentDoesNotExists);

            return await _gamesApiClient.GetGameAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
        }
    }
}