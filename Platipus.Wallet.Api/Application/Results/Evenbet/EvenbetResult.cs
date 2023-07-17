namespace Platipus.Wallet.Api.Application.Results.Evenbet;

using Base;

public sealed record EvenbetResult : BaseResult<EvenbetErrorCode>, IEvenbetResult
{
    public EvenbetResult()
    {
        Balance = default;
        ErrorDescription = string.Empty;
        Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
    }

    public EvenbetResult(
        EvenbetErrorCode errorCode,
        int balance,
        string timestamp,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        ErrorDescription = description ?? string.Empty;
        Timestamp = timestamp;
    }

    public string ErrorDescription { get; set; }

    public int Balance { get; }

    public string Timestamp { get; }
}