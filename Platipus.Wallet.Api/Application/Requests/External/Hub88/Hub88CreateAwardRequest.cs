namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs.Requests;

public record Hub88CreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    Hub88CreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<Hub88CreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IHub88GameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            Hub88CreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);

            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
               .Where(u => u.Username == apiRequest.User)
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.RewardUuid)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is not null)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            award = new Award(apiRequest.RewardUuid, apiRequest.EndTime);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.CreateAwardAsync(
                environment.BaseUrl,
                user.Casino.Params.Hub88PrivateGameServiceSecuritySign,
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