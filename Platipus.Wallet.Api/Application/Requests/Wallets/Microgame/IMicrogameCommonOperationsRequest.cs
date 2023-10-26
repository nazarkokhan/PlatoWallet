namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using System.Text.Json.Serialization;

public interface IMicrogameCommonOperationsRequest
{
    [JsonPropertyName("transactionId")]
    public string TransactionId { get; }

    [JsonPropertyName("externalGameSessionId")]
    public string ExternalGameSessionId { get; }

    [JsonPropertyName("real")]
    public decimal Real { get; }

    [JsonPropertyName("currency")]
    public string Currency { get; }

    [JsonPropertyName("roundId")]
    public long RoundId { get; }
}