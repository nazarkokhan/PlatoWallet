// ReSharper disable InconsistentNaming

namespace Platipus.Wallet.Api.Application.Results.ISoftBet;

// public record SoftBetErrorCode(string Code, SoftBetErrorType Type, string Description)
// {
//     public readonly SoftBetErrorCode R01 = new(
//         "R_01",
//         SoftBetErrorType.Request,
//         "General Request Error.");
//
//     public readonly SoftBetErrorCode R02 = new("R_02", SoftBetErrorType.Request, "Missing required parameters.");
//     public readonly SoftBetErrorCode R03 = new("R_03", SoftBetErrorType.Request, "Incorrect format of parameters.");
//     public readonly SoftBetErrorCode R04 = new("R_04", SoftBetErrorType.Request, "Configuration for given parameters does not exist.");
//     public readonly SoftBetErrorCode R05 = new("R_05", SoftBetErrorType.Request, "Player authentication failed.");
//     public readonly SoftBetErrorCode B01 = new("B_01", SoftBetErrorType.Bet, "General bet Error.");
//     public readonly SoftBetErrorCode B03 = new("B_03", SoftBetErrorType.Bet, "Insufficient Funds");
//     public readonly SoftBetErrorCode B08 = new("B_08", SoftBetErrorType.Bet, "Playing time limit has been exceeded for a player");
//     public readonly SoftBetErrorCode W01 = new("W_01", SoftBetErrorType.Win, "General win Error.");
//     public readonly SoftBetErrorCode C01 = new("C_01", SoftBetErrorType.Cancel, "General cancel Error. ");
//     public readonly SoftBetErrorCode D01 = new("D_01", SoftBetErrorType.DepositMoney, "");
//     public readonly SoftBetErrorCode D02 = new("D_02", SoftBetErrorType.DepositMoney, "");
// }
//
// public enum SoftBetErrorType
// {
//     General,
//     Request,
//     Bet,
//     Win,
//     Cancel,
//     DepositMoney
// }
// public enum SoftBetErrorCode
// {
//     R_01, //General Request Error
//     R_02, //Missing required parameters.
//     R_03, //Incorrect format of parameters.
//     R_04, //Configuration for given parameters does not exist.
//     R_05, //Player authentication failed.
//     B_01, //General bet Error.
//     B_03, //Insufficient Funds
//     B_08, //Playing time limit has been exceeded for a player
//     W_01, //General win Error.
//     C_01, //General cancel Error.
//     D_01, //General deposit Error.
//     D_02, //Wallet doesn't support deposit money method
// }

public enum SoftBetErrorMessage
{
    GeneralRequestError,
    MissingRequiredParameters,
    IncorrectFormatOfParameters,
    ConfigurationForGivenParametersDoesNotExist,
    PlayerAuthenticationFailed,
    GeneralBetError,
    InsufficientFunds,
    PlayingTimeLimitHasBeenExceededForAPlayer,
    GeneralWinError,
    GeneralCancelError,
    GeneralDepositError,
    WalletDoesntSupportDepositMoneyMethod,
}