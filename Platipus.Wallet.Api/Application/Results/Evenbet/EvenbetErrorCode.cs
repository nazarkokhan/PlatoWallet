namespace Platipus.Wallet.Api.Application.Results.Evenbet;

using System.ComponentModel;

public enum EvenbetErrorCode
{
    [Description("Insufficient funds")]
    INSUFFICIENT_FUNDS = 1,

    [Description("Invalid signature")]
    AUTHORIZATION_FAILED = 2,

    [Description("Token expired")]
    TOKEN_EXPIRED = 3,

    [Description("Unknown transaction id")]
    UNKNOWN_TRANSACTION_ID = 4,

    [Description("Invalid game")]
    INVALID_GAME = 5,

    [Description("Invalid amount")]
    INVALID_AMOUNT = 6,

    [Description("Invalid token")]
    INVALID_TOKEN = 7,

    [Description("Transaction already settled")]
    TRANSACTION_ALREADY_SETTLED = 8,

    [Description("Debit transaction does not exist")]
    DEBIT_TRANSACTION_DOES_NOT_EXIST = 9,

    [Description("Invalid parameter")]
    INVALID_PARAMETER = 400,

    [Description("Internal server error")]
    GENERAL_ERROR = 900
}