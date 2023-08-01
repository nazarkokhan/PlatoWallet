namespace Platipus.Wallet.Api.Application.Helpers.Common;

public ref struct MoneyHelper
{
    //TODO code duplication? Yiu can accept long
    public static decimal ConvertFromCents(int amount)
    {
        return amount / 100m;
    }
    
    public static decimal ConvertFromCents(long amount)
    {
        return amount / 100m;
    }
    
    public static long ConvertToCents(decimal amount)
    {
        return (long)Math.Round(amount * 100);
    }
}