namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.BetConstruct;
using Microsoft.AspNetCore.Mvc;

public class BetConstructExternalActionResult : ActionResult
{
    public BetConstructExternalActionResult(IBetConstructResult result)
    {
        Result = result;
    }

    public IBetConstructResult Result { get; }
}