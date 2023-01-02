namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using System.Net;
using Microsoft.AspNetCore.Mvc;

public record EverymatrixBalanceResponse(
    string Status,
    decimal TotalBalance,
    string Currency) : EveryMatrixBaseResponse;