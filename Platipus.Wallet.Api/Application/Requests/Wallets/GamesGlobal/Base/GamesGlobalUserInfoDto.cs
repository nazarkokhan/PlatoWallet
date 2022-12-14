namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Base;

using System.Diagnostics.CodeAnalysis;
using Horizon.XmlRpc.Core;

#pragma warning disable CS8618
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalUserInfoDto
{
    public int serverId { get; init; }
    public int userId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string? userName { get; init; } = null;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string? marketType { get; init; }
}