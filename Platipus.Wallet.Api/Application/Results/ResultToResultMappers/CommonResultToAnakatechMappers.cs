namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Anakatech;
using Anakatech.WithData;

public static class CommonResultToAnakatechMappers
{
    public static IAnakatechResult<TData> ToAnakatechFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? AnakatechResultFactory.Failure<TData>(result.Error.ToAnakatechErrorCode(), exception: result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IAnakatechResult ToAnakatechResult(this IResult result)
        => result.IsSuccess
            ? AnakatechResultFactory.Success()
            : AnakatechResultFactory.Failure(
                result.Error.ToAnakatechErrorCode(),
                0,
                false,
                exception: result.Exception);

    public static IAnakatechResult<TData> ToAnakatechResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? AnakatechResultFactory.Success(response)
            : AnakatechResultFactory.Failure<TData>(
                result.Error.ToAnakatechErrorCode(),
                exception: result.Exception);

    private static AnakatechErrorCode ToAnakatechErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.InsufficientFunds => AnakatechErrorCode.InsufficientBalance,
            ErrorCode.InvalidCurrency => AnakatechErrorCode.CurrencyNotPermitted,
            ErrorCode.RoundNotFound => AnakatechErrorCode.RoundDoesNotExist,
            ErrorCode.RoundAlreadyFinished => AnakatechErrorCode.RoundWasAlreadyClosed,
            _ => AnakatechErrorCode.InternalError
        };
    }
}