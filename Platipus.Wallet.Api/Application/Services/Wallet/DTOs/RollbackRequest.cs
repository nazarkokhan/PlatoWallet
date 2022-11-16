namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record RollbackRequest(
    Guid SessionId,
    string User,
    string Game,
    string RoundId,
    string TransactionId);