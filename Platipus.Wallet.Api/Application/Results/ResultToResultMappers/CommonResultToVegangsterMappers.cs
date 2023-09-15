namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Atlas;
using Vegangster;
using Vegangster.WithData;

public static class CommonResultToVegangsterMappers
{
    public static IVegangsterResult<TData> ToVegangsterFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? VegangsterResultFactory.Failure<TData>(result.Error.ToVegangsterResponseStatus(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IVegangsterResult ToVegangsterResult(this IResult result)
        => result.IsSuccess
            ? VegangsterResultFactory.Success()
            : VegangsterResultFactory.Failure(
                result.Error.ToVegangsterResponseStatus(),
                exception: result.Exception);

    public static IVegangsterResult<TData> ToVegangsterResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? VegangsterResultFactory.Success(response)
            : VegangsterResultFactory.Failure<TData>(
                result.Error.ToVegangsterResponseStatus(),
                exception: result.Exception);

    private static VegangsterResponseStatus ToVegangsterResponseStatus(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.TransactionAlreadyExists => VegangsterResponseStatus.ERROR_TRANSACTION_EXISTS,
            ErrorCode.TransactionNotFound => VegangsterResponseStatus.ERROR_TRANSACTION_DOES_NOT_EXIST,
            ErrorCode.UserNotFound => VegangsterResponseStatus.ERROR_INVALID_TOKEN,
            ErrorCode.UserIsDisabled => VegangsterResponseStatus.ERROR_USER_DISABLED,
            ErrorCode.InvalidCurrency => VegangsterResponseStatus.ERROR_WRONG_CURRENCY,
            ErrorCode.RoundNotFound => VegangsterResponseStatus.ERROR_ROUND_DOES_NOT_FOUND,
            _ => VegangsterResponseStatus.ERROR_GENERAL
        };
    }
}