namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record GetBalanceRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game);