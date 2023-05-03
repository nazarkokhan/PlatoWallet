namespace Platipus.Wallet.Api.Application.Requests.External.Uis;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Services.UisGamesApi;
using Services.UisGamesApi.Dto;
using Domain.Entities;
using Infrastructure.Persistence;

public record UisCreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    UisCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<UisCreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IUisGameApiClient _gameApiClient;

        public Handler(
            WalletDbContext context,
            IUisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            UisCreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentDoesNotExists);

            return await _gameApiClient.CreateAwardAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
        }
    }
}