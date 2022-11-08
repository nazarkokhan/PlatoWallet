namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base;

using Requests.Base;

public abstract record PswBaseRequest(Guid SessionId, string User): BaseRequest;