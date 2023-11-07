using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;

public interface ISweepiumBoxRequest<out TRequestData> : IBaseWalletRequest
    where TRequestData : ISweepiumRequest
{
    [JsonPropertyName("hash")]
    public string Hash { get; }
    [JsonPropertyName("time")]
    public string Time { get; }
    [JsonPropertyName("data")]
    public TRequestData Data { get; }
}