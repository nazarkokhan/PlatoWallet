namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using System.Globalization;

public record EverymatrixBalanceResponse(string TotalBalance, string Currency) : EverymatrixBaseSuccessResponse
{
    public EverymatrixBalanceResponse(decimal totalBalance, string currency)
        : this(totalBalance.ToString(CultureInfo.InvariantCulture), currency)
    {
    }
}