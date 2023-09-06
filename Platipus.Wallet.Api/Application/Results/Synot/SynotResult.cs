namespace Platipus.Wallet.Api.Application.Results.Synot;

using Base;

public sealed record SynotResult : BaseResult<SynotError>, ISynotResult
{
    public SynotResult()
    {
        ErrorDescription = string.Empty;
    }

    public SynotResult(
        SynotError error,
        Exception? exception = null,
        string? description = null)
        : base(error, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}