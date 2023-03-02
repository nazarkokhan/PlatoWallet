namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using ISoftBet;
using ISoftBet.WithData;

public static class CommonResultToSoftBetMappers
{
    public static ISoftBetResult<TData> ToSoftBetResult<TData>(this IResult result)
        => result.IsFailure
            ? SoftBetResultFactory.Failure<TData>(result.ErrorCode.ToSoftBetErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static ISoftBetResult ToSoftBetResult(this IResult result)
        => result.IsSuccess
            ? SoftBetResultFactory.Success()
            : SoftBetResultFactory.Failure(result.ErrorCode.ToSoftBetErrorCode(), result.Exception);

    private static SoftBetErrorMessage ToSoftBetErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.BadParametersInTheRequest => SoftBetErrorMessage.MissingRequiredParameters,
            // ErrorCode. => SoftBetErrorMessage.IncorrectFormatOfParameters,
            // ErrorCode. => SoftBetErrorMessage.ConfigurationForGivenParametersDoesNotExist,
            ErrorCode.UnknownAuthorizeException => SoftBetErrorMessage.PlayerAuthenticationFailed,
            ErrorCode.UnknownBetException => SoftBetErrorMessage.GeneralBetError,
            ErrorCode.InsufficientFunds => SoftBetErrorMessage.InsufficientFunds,
            // ErrorCode. => SoftBetErrorMessage.PlayingTimeLimitHasBeenExceededForAPlayer,
            ErrorCode.UnknownWinException => SoftBetErrorMessage.GeneralWinError,
            ErrorCode.UnknownRollbackException => SoftBetErrorMessage.GeneralCancelError,
            // ErrorCode. => SoftBetErrorMessage.GeneralDepositError,
            // ErrorCode. => SoftBetErrorMessage.WalletDoesntSupportDepositMoneyMethod,
            ErrorCode.Unknown or _=> SoftBetErrorMessage.GeneralRequestError,
        };
    }
}