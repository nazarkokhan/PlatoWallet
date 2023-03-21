namespace Platipus.Wallet.Api.Application.Requests.External.Reevo;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ReevoGamesApi;
using Services.ReevoGamesApi.DTO;

public record ReevoAddFreeRoundRequest(
    [property: DefaultValue("test")] string Environment,
    ReevoAddFreeRoundsGameApiRequest ApiRequest) : IRequest<IResult<object>>
{
    public class Handler : IRequestHandler<ReevoAddFreeRoundRequest, IResult<object>>
    {
        private readonly IReevoGameApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(IReevoGameApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult<object>> Handle(
            ReevoAddFreeRoundRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentDoesNotExists);

            return await _gamesApiClient.AddFreeRoundsAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
        }
    }
}