namespace Platipus.Wallet.Api.Application.Results.ISoftBet;

public static class SoftBetErrorCodeExtensions
{
    public static string ToCode(this SoftBetErrorMessage errorCode)
        => errorCode switch
        {
            SoftBetErrorMessage.GeneralRequestError => "R_01",
            SoftBetErrorMessage.MissingRequiredParameters => "R_02",
            SoftBetErrorMessage.IncorrectFormatOfParameters => "R_03",
            SoftBetErrorMessage.ConfigurationForGivenParametersDoesNotExist => "R_04",
            SoftBetErrorMessage.PlayerAuthenticationFailed => "R_05",
            SoftBetErrorMessage.GeneralBetError => "B_01",
            SoftBetErrorMessage.InsufficientFunds => "B_03",
            SoftBetErrorMessage.PlayingTimeLimitHasBeenExceededForAPlayer => "B_08",
            SoftBetErrorMessage.GeneralWinError => "W_01",
            SoftBetErrorMessage.GeneralCancelError => "C_01",
            SoftBetErrorMessage.GeneralDepositError => "D_01",
            SoftBetErrorMessage.WalletDoesntSupportDepositMoneyMethod => "D_02",
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
        };
}