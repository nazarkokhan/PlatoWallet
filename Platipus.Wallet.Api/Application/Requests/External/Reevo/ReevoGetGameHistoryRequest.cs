namespace Platipus.Wallet.Api.Application.Requests.External.Reevo;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ObsoleteGameApiStyle.ReevoGamesApi;
using Services.ObsoleteGameApiStyle.ReevoGamesApi.DTO;

public record ReevoGetGameHistoryRequest(
    [property: DefaultValue("test")] string Environment,
    ReevoGetGameHistoryGameApiRequest ApiRequest) : IRequest<IResult<object>>
{
    public class Handler : IRequestHandler<ReevoGetGameHistoryRequest, IResult<object>>
    {
        private readonly IReevoGameApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(IReevoGameApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult<object>> Handle(
            ReevoGetGameHistoryRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentNotFound);

            return await _gamesApiClient.GetGameHistoryAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
        }
    }
}