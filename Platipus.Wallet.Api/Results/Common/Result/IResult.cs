namespace Platipus.Wallet.Api.Results.Common.Result;

public interface IResult : IBaseResult<ErrorCode>
{
}

public interface IDatabetResult : IBaseResult<DatabetErrorCode>
{
}

public interface IBaseResult<out TError>
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    TError ErrorCode { get; }

    Exception? Exception { get; }

    IBaseResult<TNewError> ConvertResult<TNewError>(TNewError error);
}