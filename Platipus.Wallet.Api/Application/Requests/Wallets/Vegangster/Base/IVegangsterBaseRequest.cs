namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster.Base;

using System.Text.Json.Serialization;
using Requests.Base;

public interface IVegangsterBaseRequest : IBaseWalletRequest
{
    [JsonPropertyName("token")]
    public string Token { get; }
    
    [JsonPropertyName("player_id")]
    public string PlayerId { get; }
}