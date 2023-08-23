namespace Platipus.Wallet.Api.Application.Requests.External.Uis;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Services.UisGamesApi;
using Services.UisGamesApi.Dto;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;

[PublicAPI]
public record UisAwardBonusRequest(
    [property: DefaultValue("test")] string Environment,
    UisAwardBonusGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<UisAwardBonusRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IUisGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IUisGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            UisAwardBonusRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;
            try
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);

                // var user = await _context.Set<User>()
                //    .Where(u => u.Username == apiRequest.UserId)
                //    .Include(
                //         u => u.Awards
                //            .Where(a => a.Id == apiRequest.BonusCode))
                //    .Include(u => u.Currency)
                //    .Include(u => u.Casino)
                //    .FirstOrDefaultAsync(cancellationToken);
                // if (user is null)
                //     return ResultFactory.Failure(ErrorCode.UserNotFound);
                // if (user.Awards.Any(a => a.Id == apiRequest.BonusCode))
                //     return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);
                //
                // var expirationTime = DateTimeOffset.FromUnixTimeSeconds(apiRequest.ExpirationTimestamp)
                //    .DateTime
                //    .ToUniversalTime();
                // var award = new Award(
                //     apiRequest.q,
                //     expirationTime);
                //
                // user.Awards.Add(award);
                // _context.Update(user);
                //
                // await _context.SaveChangesAsync(cancellationToken);

                var response = await _gameApiClient.AwardBonusAsync(
                    environment.BaseUrl,
                    apiRequest,
                    cancellationToken);

                if (response is { IsSuccess: true, Data.IsSuccess: true })
                    await _context.Database.CommitTransactionAsync(cancellationToken);
                else
                    await _context.Database.RollbackTransactionAsync(cancellationToken);

                return response;
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync(cancellationToken);
                return ResultFactory.Failure(ErrorCode.Unknown);
            }
        }
    }
}