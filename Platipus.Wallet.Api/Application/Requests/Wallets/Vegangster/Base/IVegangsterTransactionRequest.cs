namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster.Base;

using System.Text.Json.Serialization;

public interface IVegangsterTransactionRequest : IVegangsterBaseRequest
{
    [JsonPropertyName("game_code")]
    public string GameCode { get; }
    
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; }
    
    [JsonPropertyName("round_id")]
    public string RoundId { get; }
}