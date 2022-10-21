namespace PlatipusWallet.Api.Application.Requests.Base.Requests;

public abstract record PswBaseRequest(Guid SessionId, string User): BaseRequest;