namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

using Application.Results.EmaraPlay;
using Humanizer;
using JetBrains.Annotations;

//TODO remove
public record EmaraPlayBaseResponse(
    string Error,
    string Description,
    Result Result);

// TODO use this response model. It is wrapper so use it only in result filter
[PublicAPI]
public abstract record EmaraPlayBaseResponseNew
{
    protected EmaraPlayBaseResponseNew(EmaraPlayErrorCode errorCode)
    {
        Error = (int)errorCode;
        Description = errorCode.Humanize();
    }

    public int Error { get; private init; } //TODO int because you json settings is to serialize number as string
    public string Description { get; private init; }
}

//TODO box this if success
public record EmaraPlayCommonBoxResponseNew<TResult>(TResult Result)
    : EmaraPlayBaseResponseNew(EmaraPlayErrorCode.Success)
    where TResult : IEmaraPlayBaseResponseNew; //TODO this is going to be response from your

//TODO box this if error
public record EmaraPlayErrorResponseNew : EmaraPlayBaseResponseNew
{
    private EmaraPlayErrorResponseNew(EmaraPlayErrorCode errorCode)
        : base(errorCode)
    {
    }
}