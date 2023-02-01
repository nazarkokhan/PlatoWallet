namespace Platipus.Wallet.Api.Application.Results.Betflag;

using Base;

public record BetflagResult : BaseResult<BetflagErrorCode>, IBetflagResult
{
    public BetflagResult()
    {
        ErrorDescription = string.Empty;
    }

    public BetflagResult(
        BetflagErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}