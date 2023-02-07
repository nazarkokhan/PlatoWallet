namespace Platipus.Wallet.Api.Application.Results.Reevo;

using Base;

public record ReevoResult : BaseResult<ReevoErrorCode>, IReevoResult
{
    public ReevoResult()
    {
        ErrorDescription = string.Empty;
    }

    public ReevoResult(
        ReevoErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}