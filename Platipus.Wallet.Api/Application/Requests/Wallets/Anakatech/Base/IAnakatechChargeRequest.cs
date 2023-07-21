namespace Platipus.Wallet.Api.Application.Requests.Wallets.Anakatech.Base;

public interface IAnakatechChargeRequest : IAnakatechBaseRequest
{
    public string RoundId { get; }

    public string TransactionId { get; }

    public string Currency { get; }

    public long Amount { get; }

    public bool CloseRound { get; }
}