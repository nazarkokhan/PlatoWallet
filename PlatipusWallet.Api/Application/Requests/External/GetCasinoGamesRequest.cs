namespace PlatipusWallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;
using Services.GamesApiService;
using Services.GamesApiService.DTOs.Responses;

public record GetCasinoGamesRequest(string CasinoId) : IRequest<IResult<GetCasinoGamesListResponseDto>>
{
    public class Handler : IRequestHandler<GetCasinoGamesRequest, IResult<GetCasinoGamesListResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IResult<GetCasinoGamesListResponseDto>> Handle(
            GetCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return ResultFactory.Failure<GetCasinoGamesListResponseDto>(ErrorCode.InvalidCasinoId);

            var casinoGamesResponse = await _gamesApiClient.GetCasinoGamesAsync(request.CasinoId, cancellationToken);

            return casinoGamesResponse;
        }
    }
}