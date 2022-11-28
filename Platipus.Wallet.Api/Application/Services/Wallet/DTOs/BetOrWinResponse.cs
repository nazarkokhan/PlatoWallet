namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record BetOrWinResponse(
    decimal Balance,
    string Currency,
    string InternalTransactionId,
    DateTime CreatedDate);