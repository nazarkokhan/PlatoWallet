namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88.Base;

using Requests.Base;

public abstract record Hub88BaseRequest(string SupplierUser, string Token, string RequestUuid) : BaseRequest;