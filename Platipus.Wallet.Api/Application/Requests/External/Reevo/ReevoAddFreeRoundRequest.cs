namespace Platipus.Wallet.Api.Application.Requests.External.Reevo;

using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Nodes;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ReevoGamesApi;
using Services.ReevoGamesApi.DTO;

public record ReevoAddFreeRoundRequest(
    [property: DefaultValue("test")] string Environment,
    ReevoAddFreeRoundsGameApiRequest ApiRequest) : IRequest<IResult<object>>
{
    public class Handler : IRequestHandler<ReevoAddFreeRoundRequest, IResult<object>>
    {
        private readonly IReevoGameApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(IReevoGameApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult<object>> Handle(
            ReevoAddFreeRoundRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure<object>(ErrorCode.EnvironmentNotFound);

            var response = await _gamesApiClient.AddFreeRoundsAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
            if (response.IsFailure)
                return response;

            var responseData = response.Data;
            if (responseData.IsFailure)
                return ResultFactory.Success(response.Data.Error);

            var validTo = DateTime.ParseExact(request.ApiRequest.ValidTo, "yyyy-MM-dd", CultureInfo.DefaultThreadCurrentCulture)
               .ToUniversalTime();

            var awardId = JsonNode.Parse(responseData.Data.Response)?["freeround_id"]?.GetValue<string>();
            if (awardId is null)
                return ResultFactory.Failure<object>(ErrorCode.InvalidExternalResponse);

            var user = await _context.Set<User>()
               .Where(e => e.Username == request.ApiRequest.PlayerIds)
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<object>(ErrorCode.UserNotFound);

            var award = new Award(awardId, validTo) { UserId = user.Id, Currency = request.ApiRequest.Currency };
            _context.Add(award);
            await _context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}