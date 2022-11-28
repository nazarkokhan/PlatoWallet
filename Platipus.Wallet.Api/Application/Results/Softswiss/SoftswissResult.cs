namespace Platipus.Wallet.Api.Application.Results.Softswiss;

using Base;

public record SoftswissResult : BaseResult<SoftswissErrorCode>, ISoftswissResult
{
    public SoftswissResult()
    {
        ErrorDescription = string.Empty;
    }

    public SoftswissResult(
        SoftswissErrorCode errorCode,
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