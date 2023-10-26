namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame.Base;

using System.Text.Json.Serialization;
using Requests.Base;

public interface IMicrogameBaseRequest : IBaseWalletRequest
{
    [JsonPropertyName("gameId")]
    public string GameId { get; }

    [JsonPropertyName("accessToken")]

    public string AccessToken { get; }
}