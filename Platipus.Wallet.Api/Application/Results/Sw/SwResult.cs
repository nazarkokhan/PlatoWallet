namespace Platipus.Wallet.Api.Application.Results.Sw;

using Base;

public record SwResult : BaseResult<SwErrorCode>, ISwResult
{
    public SwResult()
    {
        ErrorDescription = string.Empty;
    }

    public SwResult(
        SwErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}