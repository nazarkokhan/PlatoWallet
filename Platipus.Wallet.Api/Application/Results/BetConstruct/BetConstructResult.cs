namespace Platipus.Wallet.Api.Application.Results.BetConstruct;

using Base;

public record BetConstructResult : BaseResult<BetConstructErrorCode>, IBetConstructResult
{
    public BetConstructResult()
    {
        ErrorDescription = string.Empty;
    }

    public BetConstructResult(
        BetConstructErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}