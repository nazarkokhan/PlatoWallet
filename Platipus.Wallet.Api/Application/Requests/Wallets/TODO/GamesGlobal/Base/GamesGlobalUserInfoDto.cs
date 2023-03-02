namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base;

using Horizon.XmlRpc.Core;

#pragma warning disable CS8618
public record GamesGlobalUserInfoDto
{
    [XmlRpcMember("serverId")]
    public int ServerId { get; init; }

    [XmlRpcMember("userId")]
    public int UserId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("userName")]
    public string? UserName { get; init; } = null;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("marketType")]
    public string? MarketType { get; init; }
}