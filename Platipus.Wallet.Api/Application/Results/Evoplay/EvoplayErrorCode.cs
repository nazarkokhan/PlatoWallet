﻿namespace Platipus.Wallet.Api.Application.Results.Evoplay;

public enum EvoplayErrorCode
{
    E_INTERNAL = 1, 
    E_BAD_REQUEST, 
    E_DISPLAYABLE,
    E_REQUEST_TRANSPORT_FAILED, 
    E_INSUFFICIENT_BALANCE,
    E_SESSION_TOKEN_INVALID_OR_EXPIRED, 
    E_INVALID_TRANSACTION_ID,
    E_UNEXPECTED_LOGIC, 
    E_PLAYER_IS_LOCKED, 
    E_INVALID_USER_ID,
    E_PLAYER_SESSION_NOT_FOUND, 
    E_GAME_NOT_FOUND,
    E_PROVIDER_GAME_NOT_FOUND, 
    E_PROVIDER_NOT_FOUND,
    E_AGGREGATOR_NOT_FOUND
}