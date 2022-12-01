namespace Platipus.Wallet.Domain.Entities;

[Flags]
public enum ErrorMockMethod
{
    Balance,
    Bet,
    Win,
    Award,
    Rollback
}