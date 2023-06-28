namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using System.Text;
using Base;
using Microsoft.EntityFrameworkCore;
using Responses.AtlasPlatform;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Results.Atlas;
using Results.Atlas.WithData;

public sealed record AtlasAuthorizationRequest(
    string Username, string Password) : 
        IAtlasRequest, IRequest<IAtlasResult<AtlasPlatformAuthorizationResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasAuthorizationRequest, IAtlasResult<AtlasPlatformAuthorizationResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext) => 
            _walletDbContext = walletDbContext;

        public async Task<IAtlasResult<AtlasPlatformAuthorizationResponse>> Handle(
            AtlasAuthorizationRequest request, CancellationToken cancellationToken)
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
                return AtlasResultFactory.Failure<AtlasPlatformAuthorizationResponse>(
                    AtlasErrorCode.SessionValidationFailed);
            }
            
            var userDataAsBytes = Encoding.UTF8.GetBytes($"{user.Username}:{user.Password}");
            var token = Convert.ToBase64String(userDataAsBytes);
            
            var response = new AtlasPlatformAuthorizationResponse(token);
            return AtlasResultFactory.Success(response);
        }
    }

    public string? Token { get; }
}