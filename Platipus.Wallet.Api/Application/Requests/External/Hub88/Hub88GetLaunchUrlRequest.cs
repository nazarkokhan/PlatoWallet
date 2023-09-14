namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetLaunchUrlRequest(
    [property: DefaultValue("test")] string Environment,
    Hub88GetLaunchUrlGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<Hub88GetLaunchUrlRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GameApiClient _gameApiClient;

        public Handler(WalletDbContext context, IHub88GameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IResult> Handle(
            Hub88GetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
               .Where(e => e.Id == request.Environment)
               .FirstOrDefaultAsync(cancellationToken);
            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var session = await _context.Set<Session>()
               .Where(e => e.Id == apiRequest.Token)
               .FirstOrDefaultAsync(cancellationToken);
            if (session is not null)
                return ResultFactory.Failure(ErrorCode.SessionAlreadyExists);

            var user = await _context.Set<User>()
               .Where(e => e.Username == apiRequest.User)
               .Include(e => e.Casino)
               .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            session = new Session
            {
                User = user,
                IsTemporaryToken = true,
                Id = apiRequest.Token
            };
            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = await _gameApiClient.GetLaunchUrlAsync(
                environment.BaseUrl,
                user.Casino.Params.Hub88PrivateGameServiceSecuritySign,
                apiRequest,
                cancellationToken);

            if (response is not { IsSuccess: true, Data.IsSuccess: true })
            {
                _context.Remove(session);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return response;
        }
    }
}