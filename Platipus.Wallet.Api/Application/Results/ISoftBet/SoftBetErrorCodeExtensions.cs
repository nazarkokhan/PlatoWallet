namespace Platipus.Wallet.Api.Application.Results.ISoftBet;

public static class SoftBetErrorCodeExtensions
{
    public static string ToCode(this SoftBetError errorCode)
        => errorCode switch
        {
            SoftBetError.GeneralRequestError => "R_01",
            SoftBetError.MissingRequiredParameters => "R_02",
            SoftBetError.IncorrectFormatOfParameters => "R_03",
            SoftBetError.ConfigurationForGivenParametersDoesNotExist => "R_04",
            SoftBetError.PlayerAuthenticationFailed => "R_05",
            SoftBetError.GeneralBetError => "B_01",
            SoftBetError.InsufficientFunds => "B_03",
            SoftBetError.PlayingTimeLimitHasBeenExceededForAPlayer => "B_08",
            SoftBetError.GeneralWinError => "W_01",
            SoftBetError.GeneralCancelError => "C_01",
            SoftBetError.GeneralDepositError => "D_01",
            SoftBetError.WalletDoesntSupportDepositMoneyMethod => "D_02",
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
        };
}