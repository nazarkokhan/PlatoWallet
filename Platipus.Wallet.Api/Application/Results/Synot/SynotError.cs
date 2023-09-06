namespace Platipus.Wallet.Api.Application.Results.Synot;

using System.ComponentModel;

public enum SynotError
{
    [Description("Provided Api Key is not valid.")]
    INVALID_API_KEY,

    [Description("Signature is invalid.")]
    BAD_SIGNATURE,

    [Description("Token is invalid or not found.")]
    INVALID_TOKEN,

    [Description("Token has expired.")]
    TOKEN_EXPIRED,
    
    [Description("Request is in invalid format or has missing parameters.")]
    INVALID_REQUEST,

    [Description("Casino is closed.")]
    CASINO_CLOSED,

    [Description("Game is closed.")]
    GAME_CLOSED,

    [Description("Player has insufficient funds to perform a bet.")]
    INSUFFICIENT_FUNDS,

    [Description("Player has reached his limit for responsible gaming and is not able to play.")]
    RESPONSIBLE_GAMING_LIMIT_REACHED,

    [Description("Game is closed.")]
    INVALID_GAME_ROUND,

    [Description("Game round with provided Round ID was already closed.")]
    GAME_ROUND_CLOSED,

    [Description("Stake is outside limits.")]
    INVALID_STAKE,
        
    [Description("Offer not found or offer is not running.")]
    FREEGAMES_INVALID_OFFER,

    [Description("Player is not in offer or player has no spins left")]
    FREEGAMES_INVALID_PLAYER,
    
    [Description("Offer not found or invalid")]
    INVALID_OFFER,
    
    [Description("Game not found or invalid")]
    INVALID_GAME,
    
    [Description("Invalid currency.")]
    INVALID_CURRENCY,
    
    [Description("Player is in offer.")]
    PLAYER_ALREADY_IN_OFFER,

    [Description("Unspecified error occurred.")]
    UNSPECIFIED
}