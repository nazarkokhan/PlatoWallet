namespace Platipus.Wallet.Api.Application.Requests.External.Everymatrix;

using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Services.EverymatrixGameApi;
using Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;
using Domain.Entities;
using Infrastructure.Persistence;

[PublicAPI]
public record EverymatrixCreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    EverymatrixCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<EverymatrixCreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IEverymatrixGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IEverymatrixGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            EverymatrixCreateAwardRequest request,
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
               .Where(u => u.Username == apiRequest.UserId)
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.BonusId)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is not null)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            award = new Award(
                apiRequest.BonusId,
                apiRequest.FreeRoundsEndDate);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.CreateAwardAsync(
                environment.BaseUrl,
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