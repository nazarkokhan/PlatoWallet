namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using System.Text.Json.Serialization;
using JetBrains.Annotations;

[PublicAPI]
public record NemesisBaseResponse
{
    [JsonIgnore]
    public long Decimals { get; init; } = 2;
    public bool Duplicated { get; init; }
    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}