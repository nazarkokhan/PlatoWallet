namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using Requests.Base;
using Results.Everymatrix;

public record EveryMatrixBaseResponse(
    string Status,
    string Message)
{
    // Default success response
    protected EveryMatrixBaseResponse()
        : this("200", "Success")
    {
    }

}