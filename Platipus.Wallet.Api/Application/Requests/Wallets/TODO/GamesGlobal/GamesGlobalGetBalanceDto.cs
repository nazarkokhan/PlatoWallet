namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;

public record GamesGlobalGetBalanceDto
{
    [XmlRpcMember("userInfo")]
    public GamesGlobalUserInfoDto UserInfo { get; init; } = null!;

    [XmlRpcMember("productId")]
    public int ProductId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("currency")]
    public string? Currency { get; init; }
}