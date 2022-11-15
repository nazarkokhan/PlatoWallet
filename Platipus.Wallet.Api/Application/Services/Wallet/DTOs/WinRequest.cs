namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record WinRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount);