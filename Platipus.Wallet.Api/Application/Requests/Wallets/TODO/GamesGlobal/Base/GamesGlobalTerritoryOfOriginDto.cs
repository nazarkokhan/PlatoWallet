namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base;

using System.Diagnostics.CodeAnalysis;
using Horizon.XmlRpc.Core;

#pragma warning disable CS8618
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalTerritoryOfOriginDto
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string? countryName { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string? countryShortCode { get; init; }
}