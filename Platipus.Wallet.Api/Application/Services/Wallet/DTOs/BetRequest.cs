namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record BetRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount);