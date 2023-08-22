namespace Platipus.Wallet.Api.Application.Requests.External.Nemesis;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.NemesisGamesApi;
using Services.NemesisGamesApi.Requests;

public record NemesisRoundGameRequest(
    [property: DefaultValue("test")] string Environment,
    NemesisRoundGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<NemesisRoundGameRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly INemesisGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, INemesisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            NemesisRoundGameRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var response = await _gameApiClient.Round(
                environment.BaseUrl,
                apiRequest,
                cancellationToken);

            return response;
        }
    }
}