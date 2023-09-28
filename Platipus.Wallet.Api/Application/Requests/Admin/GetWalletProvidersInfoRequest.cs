namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record GetWalletProvidersInfoRequest : IRequest<IResult<List<GetWalletProvidersInfoRequest.WalletProviderInfo>>>
{
    public sealed class Handler : IRequestHandler<GetWalletProvidersInfoRequest, IResult<List<WalletProviderInfo>>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IResult<List<WalletProviderInfo>>> Handle(
            GetWalletProvidersInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletProvidersInfo = await _walletDbContext.Set<Casino>()
               .Include(c => c.Users)
               .GroupBy(c => c.Provider)
               .Select(
                    wpGroup => new WalletProviderInfo
                    {
                        WalletProviderName = wpGroup.Key.ToString(),
                        Environments = wpGroup
                           .GroupBy(c => c.GameEnvironmentId)
                           .Select(
                                envGroup => new EnvironmentInfo
                                {
                                    Name = envGroup.Key,
                                    Casinos = envGroup
                                       .Select(
                                            casino => new CasinoInfo
                                            {
                                                Id = casino.Id,
                                                Users = casino.Users.Select(u => u.Username).ToList()
                                            })
                                       .ToList()
                                })
                           .ToList()
                    })
               .ToListAsync(cancellationToken);

            return ResultFactory.Success(walletProvidersInfo);
        }
    }

    public sealed class CasinoInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("users")]
        public List<string> Users { get; set; }
    }

    public sealed class EnvironmentInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("casinos")]
        public List<CasinoInfo> Casinos { get; set; }
    }

    public sealed class WalletProviderInfo
    {
        [JsonPropertyName("walletProviderName")]
        public string WalletProviderName { get; set; }
        
        [JsonPropertyName("environments")]
        public List<EnvironmentInfo> Environments { get; set; }
    }
}