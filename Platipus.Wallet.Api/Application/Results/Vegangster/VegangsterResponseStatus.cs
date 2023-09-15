namespace Platipus.Wallet.Api.Application.Results.Vegangster;

using System.ComponentModel;

public enum VegangsterResponseStatus
{
    [Description("Successful response.")]
    OK,

    [Description("Internal error.")]
    ERROR_GENERAL,

    [Description("The data passed in request body doesn't match with the model")]
    ERROR_INVALID_DATA,
        
    [Description("Token is unknown")]
    ERROR_INVALID_TOKEN,

    [Description("Unknown game_code")]
    ERROR_INVALID_GAME,
        
    [Description("Operations in given country are unavailable")]
    ERROR_BLACKLISTED_COUNTRY,

    [Description("Transaction currency differs from Player's wallet currency.")]
    ERROR_WRONG_CURRENCY,

    [Description("Not enough money on Player's balance to place a bet.")]
    ERROR_NOT_ENOUGH_MONEY,

    [Description("Player is blocked and can't place bets.")]
    ERROR_USER_DISABLED,

    [Description("Platipus couldn't verify signature.")]
    ERROR_INVALID_SIGNATURE,

    [Description("The bet with referenced transaction_id not found.")]
    ERROR_TRANSACTION_DOES_NOT_EXIST,

    [Description("A transaction with same transaction_id was sent.")]
    ERROR_TRANSACTION_EXISTS,

    [Description("Transaction with the given round_id can't be found.")]
    ERROR_ROUND_DOES_NOT_FOUND
}