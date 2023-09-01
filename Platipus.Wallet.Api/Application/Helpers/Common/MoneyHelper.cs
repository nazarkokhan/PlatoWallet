namespace Platipus.Wallet.Api.Application.Helpers.Common;

public static class MoneyHelper
{
    public static decimal ConvertFromCents(long amount)
    {
        return amount / 100m;
    }
    
    public static long ConvertToCents(decimal amount)
    {
        return (long)Math.Round(amount * 100);
    }
}