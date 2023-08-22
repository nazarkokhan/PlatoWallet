namespace Platipus.Wallet.Api.Application.Requests.External.Nemesis;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.NemesisGamesApi;

public record NemesisCurrenciesRequest([property: DefaultValue("test")] string Environment) : IRequest<IResult>
{
    public class Handler : IRequestHandler<NemesisCurrenciesRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly INemesisGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, INemesisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            NemesisCurrenciesRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var response = await _gameApiClient.Currencies(
                environment.BaseUrl,
                cancellationToken);

            return response;
        }
    }
}