namespace Platipus.Wallet.Domain.Entities.Enums;

[Flags]
public enum MockedErrorMethod
{
    None = 0,
    Balance,
    Bet,
    Win,
    Award,
    Rollback,
    Authenticate
}