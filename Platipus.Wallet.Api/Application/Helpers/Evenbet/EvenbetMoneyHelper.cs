namespace Platipus.Wallet.Api.Application.Helpers.Evenbet;

public ref struct EvenbetMoneyHelper
{
    public static decimal ConvertToWallet(int amount)
    {
        return amount / 100m;
    }
    
    public static int ConvertFromWallet(decimal amount)
    {
        return (int)Math.Round(amount * 100);
    }
}