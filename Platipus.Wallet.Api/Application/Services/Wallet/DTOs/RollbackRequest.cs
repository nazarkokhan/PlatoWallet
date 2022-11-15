namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record RollbackRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount);