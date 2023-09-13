namespace Platipus.Wallet.Api.Application.Requests.External.Everymatrix;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Services.EverymatrixGameApi;
using Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;
using Domain.Entities;
using Infrastructure.Persistence;

public record EverymatrixDeleteFreespinRequest(
    [property: DefaultValue("test")] string Environment,
    EverymatrixDeleteAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<EverymatrixDeleteFreespinRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IEverymatrixGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IEverymatrixGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            EverymatrixDeleteFreespinRequest request,
            CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);

            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.BonusId)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is null)
                return ResultFactory.Failure(ErrorCode.AwardNotFound);

            var userIdValid = int.TryParse(apiRequest.UserId, out var userId);
            if (!userIdValid)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            if (award.UserId != userId)
                return ResultFactory.Failure(ErrorCode.AwardDoesNotBelongToThisUser);

            _context.Remove(award);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.DeleteAwardAsync(
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