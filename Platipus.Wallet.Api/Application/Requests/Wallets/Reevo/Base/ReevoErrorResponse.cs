namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

using Results.Reevo;

public record ReevoErrorResponse(
    int Status,
    string Msg)
{
    public ReevoErrorResponse(ReevoErrorCode status)
        : this(
            (int)status,
            status.ToString())
    {
    }
}