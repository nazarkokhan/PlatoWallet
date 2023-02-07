namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

using System.Globalization;
using Results.Reevo;

public record ReevoSuccessResponse(
    string Status,
    string Balance)
{
    public ReevoSuccessResponse(decimal balance)
        : this(
            ((int)ReevoErrorCode.Success).ToString(),
            balance.ToString(CultureInfo.InvariantCulture))
    {
    }
}