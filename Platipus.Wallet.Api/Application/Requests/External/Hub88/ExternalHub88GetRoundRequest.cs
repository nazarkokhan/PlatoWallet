namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Hub88;
using Results.Hub88.WithData;
using Services.ObsoleteGameApiStyle.Hub88GamesApi;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Responses;

public record ExternalHub88GetRoundRequest(string TransactionUuid) : IRequest<IHub88Result<Hub88GetRoundGamesApiResponseDto>>
{
    public class Handler : IRequestHandler<ExternalHub88GetRoundRequest, IHub88Result<Hub88GetRoundGamesApiResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IHub88GamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IHub88Result<Hub88GetRoundGamesApiResponseDto>> Handle(
            ExternalHub88GetRoundRequest request,
            CancellationToken cancellationToken)
        {
            var transaction = await _context.Set<Transaction>()
                .Where(c => c.Id == request.TransactionUuid)
                .Select(
                    c => new
                    {
                        c.Id,
                        c.RoundId,
                        UserName = c.Round.User.Username,
                        c.Round.User.CasinoId
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
                return Hub88ResultFactory.Failure<Hub88GetRoundGamesApiResponseDto>(
                    Hub88ErrorCode.RS_ERROR_TRANSACTION_DOES_NOT_EXIST);

            var hub88GetRoundGamesApiRequestDto = transaction.Map(
                t => new Hub88GetRoundGamesApiRequestDto(
                    t.UserName,
                    t.Id,
                    t.RoundId,
                    t.CasinoId));
            var casinoGamesResponse = await _gamesApiClient.GetRoundAsync(
                hub88GetRoundGamesApiRequestDto,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}