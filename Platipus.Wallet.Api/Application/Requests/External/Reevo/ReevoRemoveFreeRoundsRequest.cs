namespace Platipus.Wallet.Api.Application.Requests.External.Reevo;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ReevoGamesApi;
using Services.ReevoGamesApi.DTO;

public record ReevoRemoveFreeRoundsRequest(
    [property: DefaultValue("test")] string Environment,
    ReevoRemoveFreeRoundsGameApiRequest ApiRequest) : IRequest<IResult<object>>
{
    public class Handler : IRequestHandler<ReevoRemoveFreeRoundsRequest, IResult<object>>
    {
        private readonly IReevoGameApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(IReevoGameApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult<object>> Handle(
            ReevoRemoveFreeRoundsRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var response = await _gamesApiClient.RemoveFreeRoundsAsync(environment.BaseUrl, apiRequest, cancellationToken);
            if (response.IsFailure)
                return response;

            var data = response.Data;
            if (data.ErrorMessage is not null)
                return ResultFactory.Success(data.ErrorMessage);

            await _context.Set<Award>()
                .Where(e => e.Id == apiRequest.FreeroundId)
                .ExecuteDeleteAsync(cancellationToken);

            return ResultFactory.Success(data.Success);
        }
    }
}