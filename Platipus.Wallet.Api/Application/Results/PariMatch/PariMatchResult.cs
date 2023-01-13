namespace Platipus.Wallet.Api.Application.Results.PariMatch;

using Base;

public record PariMatchResult : BaseResult<PariMatchErrorCode>, IPariMatchResult
{
    public PariMatchResult()
    {
        ErrorDescription = string.Empty;
    }

    public PariMatchResult(
        PariMatchErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}