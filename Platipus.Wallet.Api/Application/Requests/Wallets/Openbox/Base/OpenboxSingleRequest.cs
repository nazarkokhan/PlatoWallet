namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using Requests.Base;

public record OpenboxSingleRequest(
    Guid Type,
    Guid Method,
    Guid VendorUid,
    DateTime Timestamp,
    string Payload) : BaseRequest;