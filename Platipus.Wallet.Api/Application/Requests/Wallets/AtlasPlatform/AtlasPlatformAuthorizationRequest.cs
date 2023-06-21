using System.Text;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;

public sealed record AtlasPlatformAuthorizationRequest(
    string Username, string Password) : 
        IAtlasPlatformRequest, IRequest<IAtlasPlatformResult<AtlasPlatformAuthorizationResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasPlatformAuthorizationRequest, IAtlasPlatformResult<AtlasPlatformAuthorizationResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => 
            _walletDbContext = walletDbContext;

        public async Task<IAtlasPlatformResult<AtlasPlatformAuthorizationResponse>> Handle(
            AtlasPlatformAuthorizationRequest request, CancellationToken cancellationToken)
        {
            var user = await _walletDbContext.Set<User>()
                .Include(c => c.Casino)
                .Select(u => new
                {
                    u.Username,
                    u.Password,
                    CasinoProvider = u.Casino.Provider
                })
                .Where(u => u.Username == request.Username &&
                            u.Password == request.Password && 
                            u.CasinoProvider == CasinoProvider.Atlas)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            if (user is null)
            {
                return AtlasPlatformResultFactory.Failure<AtlasPlatformAuthorizationResponse>(
                    AtlasPlatformErrorCode.SessionValidationFailed);
            }
            
            var userDataAsBytes = Encoding.UTF8.GetBytes($"{user.Username}:{user.Password}");
            var token = Convert.ToBase64String(userDataAsBytes);
            
            var response = new AtlasPlatformAuthorizationResponse(token);
            return AtlasPlatformResultFactory.Success(response);
        }
    }

    public string? Token { get; }
}