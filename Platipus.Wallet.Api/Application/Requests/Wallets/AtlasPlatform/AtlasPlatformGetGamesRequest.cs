using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformGetGamesRequest(
    string? Token = null, 
    string? CasinoId = null) : 
        IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformGetGamesResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasPlatformGetGamesRequest, IAtlasPlatformResult<AtlasPlatformGetGamesResponse>>
    {
        public async Task<IAtlasPlatformResult<AtlasPlatformGetGamesResponse>> Handle(
            AtlasPlatformGetGamesRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}