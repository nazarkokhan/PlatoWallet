namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record WalletGetBalanceResponse(
    int UserId,
    string Username,
    decimal Balance,
    string Currency,
    string CasinoId,
    bool IsDisabled);