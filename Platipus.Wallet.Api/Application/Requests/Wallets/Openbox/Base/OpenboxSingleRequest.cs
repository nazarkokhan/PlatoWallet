namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using Requests.Base;

public record OpenboxSingleRequest(
    string Type,
    string Method,
    string VendorUid,
    DateTime Timestamp,
    string Payload) : BaseRequest;