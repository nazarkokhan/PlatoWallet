namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.PswGameApi;
using Services.PswGameApi.Requests;

public record PswGameBuyRequest(
    [property: DefaultValue("test")] string Environment,
    bool IsBetflag,
    PswGameBuyGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<PswGameBuyRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IPswGameApiClient _gameApiClient;

        public Handler(
            WalletDbContext context,
            IPswGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            PswGameBuyRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
               .Where(e => e.Username == apiRequest.User)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var response = await _gameApiClient.GameBuyAsync(
                environment.BaseUrl,
                apiRequest,
                request.IsBetflag,
                cancellationToken);
            if (response.IsFailure || response.Data.IsFailure)
                return response;
            var responseRoundId = response.Data.Data.RoundId.ToString();

            var round = await _context.Set<Round>()
               .Where(r => r.Id == responseRoundId)
               .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            round = new Round(responseRoundId) { User = user };
            _context.Add(round);

            await _context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}