namespace Platipus.Wallet.Api.Application.Results.BetConstruct;

using Base;

public record BetconstructResult : BaseResult<BetconstructErrorCode>, IBetconstructResult
{
    public BetconstructResult()
    {
        ErrorDescription = string.Empty;
    }

    public BetconstructResult(
        BetconstructErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}