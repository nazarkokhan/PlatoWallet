namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record BetOrWinResponse2(
    decimal Balance,
    string Currency,
    string InternalTransactionId,
    DateTime CreatedDate);