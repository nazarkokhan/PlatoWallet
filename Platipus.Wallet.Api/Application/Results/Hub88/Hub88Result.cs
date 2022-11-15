namespace Platipus.Wallet.Api.Application.Results.Hub88;

using Base;

public record Hub88Result : BaseResult<Hub88ErrorCode>, IHub88Result
{
    public Hub88Result()
    {
        ErrorDescription = string.Empty;
    }

    public Hub88Result(
        Hub88ErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}