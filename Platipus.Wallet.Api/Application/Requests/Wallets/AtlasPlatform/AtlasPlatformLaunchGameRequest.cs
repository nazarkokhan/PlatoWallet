using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformLaunchGameRequest(
    string GameId, bool Demo, bool IsMobile, string Token,
    string Language, string CashierUrl, string LobbyUrl,
    string? CasinoId = null) : 
        IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformLaunchGameResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasPlatformLaunchGameRequest, IAtlasPlatformResult<AtlasPlatformLaunchGameResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IAtlasPlatformResult<AtlasPlatformLaunchGameResponse>> Handle(
            AtlasPlatformLaunchGameRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}