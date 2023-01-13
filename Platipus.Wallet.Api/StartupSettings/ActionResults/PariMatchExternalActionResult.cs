namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.PariMatch;
using Microsoft.AspNetCore.Mvc;

public class PariMatchExternalActionResult : ActionResult
{
    public PariMatchExternalActionResult(IPariMatchResult result)
    {
        Result = result;
    }

    public IPariMatchResult Result { get; }
}