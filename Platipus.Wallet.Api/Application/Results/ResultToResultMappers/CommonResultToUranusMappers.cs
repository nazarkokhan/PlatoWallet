namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Uranus;
using Uranus.WithData;

public static class CommonResultToUranusMappers
{
    public static IUranusResult<TData> ToUranusFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? UranusResultFactory.Failure<TData>(result.Error.ToUranusErrorCode(), exception: result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IUranusResult ToUranusResult(this IResult result)
        => result.IsSuccess
            ? UranusResultFactory.Success()
            : UranusResultFactory.Failure(result.Error.ToUranusErrorCode(), "0", "EUR",
                exception: result.Exception);

    public static IUranusResult<TData> ToUranusResult<TData>(
        this IResult result, TData response)
        => result.IsSuccess
            ? UranusResultFactory.Success(response)
            : UranusResultFactory.Failure<TData>(result.Error.ToUranusErrorCode(), 
                exception: result.Exception);

    private static UranusErrorCode ToUranusErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => UranusErrorCode.E_PROVIDER_NOT_FOUND,
            ErrorCode.TransactionAlreadyExists => UranusErrorCode.E_INVALID_TRANSACTION_ID,
            ErrorCode.UserNotFound => UranusErrorCode.E_INVALID_USER_ID,
            ErrorCode.UserIsDisabled => UranusErrorCode.E_PLAYER_IS_LOCKED,
            ErrorCode.RoundAlreadyExists => UranusErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.RoundAlreadyFinished => UranusErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.InvalidCurrency => UranusErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.RoundNotFound => UranusErrorCode.E_UNEXPECTED_LOGIC,
            _ => UranusErrorCode.E_INTERNAL
        };
    }
}