namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetGameListRequest(
    [property: DefaultValue("test")] string Environment,
    Hub88GetGameListGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<Hub88GetGameListRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IHub88GameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            Hub88GetGameListRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var casino = await _context.Set<Casino>()
               .Where(u => u.Id == request.ApiRequest.OperatorId)
               .FirstOrDefaultAsync(cancellationToken);
            if (casino is null)
                return ResultFactory.Failure(ErrorCode.CasinoNotFound);

            var response = await _gameApiClient.GetGameListAsync(
                environment.BaseUrl,
                casino.Params.Hub88PrivateGameServiceSecuritySign,
                request.ApiRequest,
                cancellationToken);

            return response;
        }
    }
}