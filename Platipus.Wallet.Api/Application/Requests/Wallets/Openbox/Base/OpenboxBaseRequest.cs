namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using Requests.Base;

public abstract record OpenboxBaseRequest(string Token) : BaseRequest;