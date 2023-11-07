using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;

public interface ISweepiumRequest : IBaseWalletRequest
{
    [JsonPropertyName("token")]
    public string Token { get; }
}