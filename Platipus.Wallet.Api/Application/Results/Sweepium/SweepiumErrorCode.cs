namespace Platipus.Wallet.Api.Application.Results.Sweepium;

using System.ComponentModel;

public enum SweepiumErrorCode
{
    [Description("Wrong Bet Amount")]
    Wrong_Bet_Amount = 4,
    
    [Description("Session not enabled")]
    Session_Not_Enabled = 5,

    [Description("Data not found")]
    Data_Not_Found = 6,

    [Description("Wrong Game ID")]
    Wrong_Game_ID = 7,

    [Description("Authentication Failed")]
    Authentication_Failed = 8,
    
    [Description("Session not found")]
    Session_Not_Found = 9,

    [Description("Not Enough Money")]
    Not_Enough_Money = 21,

    [Description("Player Is Blocked")]
    Player_Is_Blocked = 29,

    [Description("Wrong Currency")]
    Wrong_Currency = 34,

    [Description("Wrong Win Amount")]
    Wrong_Win_Amount = 63,

    [Description("Game is Blocked")]
    Game_Is_Blocked = 84,

    [Description("Transaction already exists")]
    Transaction_AlreadyExists = 104,
    
    [Description("The transaction is already canceled")]
    Transaction_Is_Already_Canceled = 105,

    [Description("Token Expired")]
    Token_Expired = 106,
    
    [Description("Transaction not found")]
    Transaction_Not_Found = 107,

    [Description("Token not Found")]
    Token_Not_Found = 114,

    [Description("General Error")]
    General_Error = 130,

    [Description("Incorrect Parameters Passed")]
    Incorrect_Parameters_Passed = 200,

    [Description("Insufficient Funds")]
    Insufficient_Funds = 500
}