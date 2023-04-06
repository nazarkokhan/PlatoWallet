namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

using Base;

public record EmaraPlayResult : BaseResult<EmaraPlayErrorCode>, IEmaraPlayResult
{
    public EmaraPlayResult()
    {
        ErrorDescription = string.Empty;
    }

    public EmaraPlayResult(
        EmaraPlayErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }

    public long? Balance { get; }
}