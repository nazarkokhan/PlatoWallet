using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Models;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformGetGamesRequest(
    string? Token = null, string? CasinoId = "") : 
        IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformGetGamesResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasPlatformGetGamesRequest, IAtlasPlatformResult<AtlasPlatformGetGamesResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => 
            _walletDbContext = walletDbContext;

        public async Task<IAtlasPlatformResult<AtlasPlatformGetGamesResponse>> Handle(
            AtlasPlatformGetGamesRequest request, CancellationToken cancellationToken)
        {
            var casinoId = request.CasinoId;
    
            var casinoGamesQuery = _walletDbContext.Set<CasinoGames>()
                .Include(g => g.Game)
                .Include(g => g.Casino);

            IQueryable<CasinoGames> filteredCasinoGamesQuery;

            if (string.IsNullOrWhiteSpace(casinoId))
            {
                filteredCasinoGamesQuery = casinoGamesQuery
                    .Where(cg => cg.Casino.Provider == CasinoProvider.AtlasPlatform);
            }
            else
            {
                filteredCasinoGamesQuery = casinoGamesQuery
                    .Where(cg => cg.CasinoId == casinoId);
            }

            var games = await filteredCasinoGamesQuery
                .Select(g => new AtlasPlatformGameModel(
                    g.GameId.ToString(),
                    g.Game.Name,
                    g.Game.CategoryId.ToString(),
                    true,  // DemoGameAvailable - modify this as per your business logic
                    false,  // JackpotAvailable - modify this as per your business logic
                    false,  // IsFreeSpinAvailable - modify this as per your business logic
                    true,   // IsDesktop - modify this as per your business logic
                    true    // IsMobile - modify this as per your business logic
                ))
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new AtlasPlatformGetGamesResponse(games);
            return AtlasPlatformResultFactory.Success(response);
        }
    }
}