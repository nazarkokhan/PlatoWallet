namespace Platipus.Wallet.Api.Application.Requests.External.Parimatch;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ParimatchGameApi;
using Services.ParimatchGameApi.Requests;

public record ParimatchDeleteAwardRequest(
    [property: DefaultValue("test")] string Environment,
    ParimatchDeleteAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<ParimatchDeleteAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IParimatchGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IParimatchGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            ParimatchDeleteAwardRequest request,
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
               .Where(a => a.Id == apiRequest.GiftId)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is null)
                return ResultFactory.Failure(ErrorCode.AwardNotFound);

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