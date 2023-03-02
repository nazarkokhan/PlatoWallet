namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;

public record GamesGlobalCancelFundGameDto
{
    [XmlRpcMember("userInfo")]
    public GamesGlobalUserInfoDto UserInfo { get; init; } = null!;

    [XmlRpcMember("requestItemId")]
    public string RequestItemId { get; init; } = null!;

    [XmlRpcMember("externalProductId")]
    public int ExternalProductId { get; init; }
}