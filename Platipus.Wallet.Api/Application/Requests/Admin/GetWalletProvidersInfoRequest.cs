namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.Text.Json.Serialization;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

public sealed record
    GetWalletProvidersInfoRequest : IRequest<IResult<List<GetWalletProvidersInfoRequest.WalletProviderInfo>>>
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
               .GroupBy(c => c.Provider)
               .Select(
                    pg => new WalletProviderInfo(
                        pg.Key.ToString(),
                        pg
                           .SelectMany(с => с.CasinoGameEnvironments)
                           .GroupBy(c => c.GameEnvironmentId)
                           .Select(
                                eg => new EnvironmentInfo(
                                    eg.Key,
                                    eg
                                       .Select(e => e.Casino)
                                       .Select(
                                            c => new CasinoInfo(
                                                c.Id,
                                                c.Users
                                                   .Select(u => u.Username)
                                                   .ToList()))
                                       .ToList()))
                           .ToList()))
               .ToListAsync(cancellationToken);

            return ResultFactory.Success(walletProvidersInfo);
        }
    }

    [PublicAPI]
    public sealed record CasinoInfo(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("users")] List<string> Users);

    [PublicAPI]
    public sealed record EnvironmentInfo(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("casinos")] List<CasinoInfo> Casinos);

    [PublicAPI]
    public sealed record WalletProviderInfo(
        [property: JsonPropertyName("walletProviderName")] string WalletProviderName,
        [property: JsonPropertyName("environments")] List<EnvironmentInfo> Environments);
}