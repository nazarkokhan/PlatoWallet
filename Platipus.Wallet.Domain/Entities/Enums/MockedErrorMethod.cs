namespace Platipus.Wallet.Domain.Entities.Enums;

[Flags]
public enum MockedErrorMethod
{
    Balance,
    Bet,
    Win,
    Award,
    Rollback
}