namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.PswGamesApi;
using Services.PswGamesApi.DTOs.Requests;

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

            var response = await _gameApiClient.GameSessionAsync(
                environment.BaseUrl,
                request.ApiRequest,
                request.LaunchModeType,
                request.IsBetflag,
                cancellationToken);

            return response;
        }
    }
}