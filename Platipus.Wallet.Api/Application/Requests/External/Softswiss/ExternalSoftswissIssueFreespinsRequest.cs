namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.SoftswissGamesApi;
using Services.SoftswissGamesApi.DTOs.Requests;

public record ExternalSoftswissIssueFreespinsRequest(
        [property: DefaultValue("test")] string Environment,
        SoftswissIssueFreespinsGameApiRequest ApiRequest)
    : IRequest<IResult>
{
    public class Handler : IRequestHandler<ExternalSoftswissIssueFreespinsRequest, IResult>
    {
        private readonly ISoftswissGamesApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(ISoftswissGamesApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult> Handle(
            ExternalSoftswissIssueFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentDoesNotExists);
            var apiRequest = request.ApiRequest;

            var response = await _gamesApiClient.IssueFreespinsAsync(environment.BaseUrl, apiRequest, cancellationToken);
            if (response.IsFailure)
                return response;

            var user = await _context.Set<User>()
                .Where(e => e.Username == apiRequest.User.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<object>(ErrorCode.UserNotFound);

            var award = new Award(apiRequest.IssueId, apiRequest.ValidUntil) { UserId = user.Id };
            _context.Add(award);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}