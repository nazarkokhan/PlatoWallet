namespace Platipus.Wallet.Api.Application.Helpers.Common;

public ref struct MoneyHelper
{
    public static decimal ConvertFromCents(int amount)
    {
        return amount / 100m;
    }
    
    public static int ConvertToCents(decimal amount)
    {
        return (int)Math.Round(amount * 100);
    }
}