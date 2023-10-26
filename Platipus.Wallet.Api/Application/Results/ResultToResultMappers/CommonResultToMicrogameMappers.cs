namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Microgame;
using Microgame.WithData;

public static class CommonResultToMicrogameMappers
{
    public static IMicrogameResult<TData> ToMicrogameErrorResult<TData>(this IResult result)
        => result.IsFailure
            ? MicrogameResultFactory.Failure<TData>(result.Error.ToMicrogameStatusCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IMicrogameResult ToMicrogameResult(this IResult result)
        => result.IsSuccess
            ? MicrogameResultFactory.Success()
            : MicrogameResultFactory.Failure(
                result.Error.ToMicrogameStatusCode(),
                exception: result.Exception);

    public static IMicrogameResult<TData> ToMicrogameResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? MicrogameResultFactory.Success(response)
            : MicrogameResultFactory.Failure<TData>(
                result.Error.ToMicrogameStatusCode(),
                exception: result.Exception);

    private static MicrogameStatusCode ToMicrogameStatusCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.TransactionAlreadyExists => MicrogameStatusCode.DUPLICATETRANSACTION,
            ErrorCode.UserNotFound => MicrogameStatusCode.NOUSER,
            ErrorCode.UserIsDisabled => MicrogameStatusCode.ACCOUNTDISABLED,
            ErrorCode.InvalidCurrency => MicrogameStatusCode.INVALIDCURRENCY,
            ErrorCode.RoundNotFound => MicrogameStatusCode.INTERNALERROR,
            _ => MicrogameStatusCode.GENERICERROR
        };
    }
}