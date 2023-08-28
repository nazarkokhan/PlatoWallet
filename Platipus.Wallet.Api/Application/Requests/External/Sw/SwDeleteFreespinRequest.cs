namespace Platipus.Wallet.Api.Application.Requests.External.Sw;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.SwGameApi;

public record SwDeleteFreespinRequest(
    [property: DefaultValue("test")] string Environment,
    SwDeleteFreespinGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<SwDeleteFreespinRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly ISwGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, ISwGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            SwDeleteFreespinRequest request,
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
               .Where(a => a.Id == apiRequest.FreespinId)
               .Include(a => a.User)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is null)
                return ResultFactory.Failure(ErrorCode.AwardNotFound);

            var keyValid = SwKeyData.TryParse(apiRequest.Key, out var keyData);
            if (!keyValid)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            if (award.User.CasinoId != keyData!.CasinoId)
                return ResultFactory.Failure(ErrorCode.AwardDoesNotBelongToThisUser);

            _context.Remove(award);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.DeleteFreespin(
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