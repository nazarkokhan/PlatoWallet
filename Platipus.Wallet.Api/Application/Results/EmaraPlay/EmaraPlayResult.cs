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
        long? balance = null,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }

    public long? Balance { get; }
}