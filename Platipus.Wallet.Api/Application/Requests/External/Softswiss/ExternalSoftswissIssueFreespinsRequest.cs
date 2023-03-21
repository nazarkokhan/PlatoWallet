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

            return await _gamesApiClient.IssueFreespinsAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);
        }
    }
}