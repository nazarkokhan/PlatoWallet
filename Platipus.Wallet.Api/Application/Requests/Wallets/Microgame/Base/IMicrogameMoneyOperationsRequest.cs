namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame.Base;

using System.Text.Json.Serialization;

public interface IMicrogameMoneyOperationsRequest
{
    [JsonPropertyName("externalId")]
    public string ExternalId { get; }
}