namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisBaseResponse
{
    public long Decimals { get; init; } = 2;
    public bool Duplicated { get; init; } = false;
    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}