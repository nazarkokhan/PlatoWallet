namespace Platipus.Wallet.Api.Application.Requests.External.Sw;

using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.SwGameApi;

[PublicAPI]
public record SwCreateFreespinRequest(
    [property: DefaultValue("test")] string Environment,
    SwCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<SwCreateFreespinRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly ISwGameApiClient _gameApiClient;

        public Handler(WalletDbContext context, ISwGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            SwCreateFreespinRequest request,
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
               .Where(u => u.Id == Convert.ToInt32(apiRequest.UserId))
               .Include(u => u.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var award = await _context.Set<Award>()
               .Where(a => a.Id == apiRequest.FreespinId)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is not null)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            var validUntil = DateTime.Parse(apiRequest.Expire).ToUniversalTime();
            award = new Award(
                apiRequest.FreespinId,
                validUntil);

            user.Awards.Add(award);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.CreateFreespin(
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