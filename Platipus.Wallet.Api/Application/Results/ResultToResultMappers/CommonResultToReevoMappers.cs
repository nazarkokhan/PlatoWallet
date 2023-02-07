namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Reevo;
using Reevo.WithData;

public static class CommonResultToReevoMappers
{
    public static IReevoResult<TData> ToReevoResult<TData>(this IResult result)
        => result.IsFailure
            ? ReevoResultFactory.Failure<TData>(result.ErrorCode.ToReevoErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IReevoResult ToReevoResult(this IResult result)
        => result.IsSuccess
            ? ReevoResultFactory.Success()
            : ReevoResultFactory.Failure(result.ErrorCode.ToReevoErrorCode(), result.Exception);

    public static ReevoErrorCode ToReevoErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.Unknown or _ => ReevoErrorCode.GeneralError
        };
    }
}