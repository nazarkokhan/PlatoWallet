namespace Platipus.Wallet.Api.Application.Results.Everymatrix;

using Base;

public record EverymatrixResult : BaseResult<EverymatrixErrorCode>, IEverymatrixResult
{
    public EverymatrixResult()
    {
        ErrorDescription = string.Empty;
    }

    public EverymatrixResult(
        EverymatrixErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}