namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;
using Services.PswGamesApi;
using Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Responses;
using Domain.Entities;
using Infrastructure.Persistence;

public record PswGameBuyRequest(
    [property: DefaultValue("test")] string Environment,
    PswGameBuyGamesApiRequest ApiRequest) : IRequest<IPswResult<PswGameBuyGamesApiResponseDto>>
{
    public class Handler : IRequestHandler<PswGameBuyRequest, IPswResult<PswGameBuyGamesApiResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IPswAndBetflagGameApiClient _gameApiClient;

        public Handler(
            WalletDbContext context,
            IPswAndBetflagGameApiClient gameApiClient)
        {
            _context = context;
            _gameApiClient = gameApiClient;
        }

        public async Task<IPswResult<PswGameBuyGamesApiResponseDto>> Handle(
            PswGameBuyRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return PswResultFactory.Failure<PswGameBuyGamesApiResponseDto>(PswErrorCode.EnvironmentNotFound);

            var apiRequest = request.ApiRequest;

            var user = await _context.Set<User>()
                .Where(e => e.Username == apiRequest.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure<PswGameBuyGamesApiResponseDto>(PswErrorCode.InvalidUser);

            var response = await _gameApiClient.GameBuyAsync(apiRequest, cancellationToken);

            if (response.IsFailure)
                return response;
            var responseRoundId = response.Data.RoundId.ToString();

            var round = await _context.Set<Round>()
                .Where(r => r.Id == responseRoundId)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return PswResultFactory.Failure<PswGameBuyGamesApiResponseDto>(PswErrorCode.Duplication);

            round = new Round(responseRoundId) { User = user };
            _context.Add(round);

            await _context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}