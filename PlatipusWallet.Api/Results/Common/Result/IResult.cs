namespace PlatipusWallet.Api.Results.Common.Result;

public interface IResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    public ErrorCode ErrorCode { get; }

    Exception? Exception { get; }
}