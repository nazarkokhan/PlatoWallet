namespace Platipus.Wallet.Api.Application.Results.ISoftBet;

using Base;

public record SoftBetResult : BaseResult<SoftBetError>, ISoftBetResult
{
    public SoftBetResult()
    {
        ErrorDescription = string.Empty;
    }

    public SoftBetResult(
        SoftBetError errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}