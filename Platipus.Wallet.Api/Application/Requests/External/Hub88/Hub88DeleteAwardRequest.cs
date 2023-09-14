namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs.Requests;

public record Hub88DeleteAwardRequest(
    [property: DefaultValue("test")] string Environment,
    Hub88DeleteAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<Hub88DeleteAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IHub88GameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            Hub88DeleteAwardRequest request,
            CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);

            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var casino = await _context.Set<Casino>()
               .Where(u => u.Id == apiRequest.OperatorId)
               .FirstOrDefaultAsync(cancellationToken);
            if (casino is null)
                return ResultFactory.Failure(ErrorCode.CasinoNotFound);

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.RewardUuid)
               .Include(a => a.User)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is null)
                return ResultFactory.Failure(ErrorCode.AwardNotFound);
            if (award.User.CasinoId != casino.Id)
                return ResultFactory.Failure(ErrorCode.AwardDoesNotBelongToThisCasino);

            _context.Remove(award);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.DeleteAwardAsync(
                environment.BaseUrl,
                casino.Params.Hub88PrivateGameServiceSecuritySign,
                apiRequest,
                cancellationToken);

            if (response is { IsSuccess: true, Data.IsSuccess: true })
                await _context.Database.CommitTransactionAsync(cancellationToken);
            else
                await _context.Database.RollbackTransactionAsync(cancellationToken);

            return response;
        }
    }
}