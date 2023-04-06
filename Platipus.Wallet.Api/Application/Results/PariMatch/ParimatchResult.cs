namespace Platipus.Wallet.Api.Application.Results.PariMatch;

using Base;

public record ParimatchResult : BaseResult<ParimatchErrorCode>, IParimatchResult
{
    public ParimatchResult()
    {
        ErrorDescription = string.Empty;
    }

    public ParimatchResult(
        ParimatchErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}