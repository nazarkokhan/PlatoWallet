namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot.Base;

using System.Text.Json.Serialization;

public interface ISynotOperationsRequest : ISynotBaseRequest
{
    public long Id { get; }
    
    [JsonPropertyName("roundId")]
    public long RoundId { get; }
    
    public long Amount { get; }
}