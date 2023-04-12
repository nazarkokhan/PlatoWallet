namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.Psw;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Services.PswGamesApi;
using Services.PswGamesApi.DTOs.Responses;

public record GetPswCasinoGamesRequest(string CasinoId) : IRequest<IPswResult<PswGetCasinoGamesListGamesApiResponseDto>>
{
    public class Handler : IRequestHandler<GetPswCasinoGamesRequest, IPswResult<PswGetCasinoGamesListGamesApiResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IPswAndBetflagGameApiClient _pswAndBetflagGameApiClient;

        public Handler(WalletDbContext context, IPswAndBetflagGameApiClient pswAndBetflagGameApiClient)
        {
            _context = context;
            _pswAndBetflagGameApiClient = pswAndBetflagGameApiClient;
        }

        public async Task<IPswResult<PswGetCasinoGamesListGamesApiResponseDto>> Handle(
            GetPswCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(
                    c => c.Id == request.CasinoId
                      && c.Provider == CasinoProvider.Psw)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return PswResultFactory.Failure<PswGetCasinoGamesListGamesApiResponseDto>(PswErrorCode.InvalidCasinoId);

            var casinoGamesResponse = await _pswAndBetflagGameApiClient.GetCasinoGamesAsync(request.CasinoId, cancellationToken);

            return casinoGamesResponse;
        }
    }
}