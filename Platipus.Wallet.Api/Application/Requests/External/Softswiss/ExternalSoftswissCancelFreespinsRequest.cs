namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Services.ObsoleteGameApiStyle.SoftswissGamesApi;
using Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

public record ExternalSoftswissCancelFreespinsRequest(
        string Environment,
        SoftswissCancelFreespinsGameApiRequest ApiRequest)
    : IRequest<IResult>
{
    public class Handler : IRequestHandler<ExternalSoftswissCancelFreespinsRequest, IResult>
    {
        private readonly ISoftswissGamesApiClient _gamesApiClient;
        private readonly WalletDbContext _context;

        public Handler(ISoftswissGamesApiClient gamesApiClient, WalletDbContext context)
        {
            _gamesApiClient = gamesApiClient;
            _context = context;
        }

        public async Task<IResult> Handle(
            ExternalSoftswissCancelFreespinsRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var response = await _gamesApiClient.CancelFreespinsAsync(environment.BaseUrl, request.ApiRequest, cancellationToken);

            var data = response.Data;
            if (data.Content is null)
                return response;

            await _context.Set<Award>()
                .Where(e => e.Id == request.ApiRequest.IssueId)
                .ExecuteDeleteAsync(cancellationToken);

            return response;
        }
    }
}