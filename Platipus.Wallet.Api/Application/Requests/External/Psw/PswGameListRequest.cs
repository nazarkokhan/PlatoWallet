namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.PswGameApi;
using Services.PswGameApi.Requests;

public record PswGameListRequest(
    [property: DefaultValue("test")] string Environment,
    bool IsBetflag,
    PswGameListGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<PswGameListRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IPswGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IPswGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            PswGameListRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var response = await _gameApiClient.GameListAsync(
                environment.BaseUrl,
                request.ApiRequest,
                request.IsBetflag,
                cancellationToken);

            return response;
        }
    }
}