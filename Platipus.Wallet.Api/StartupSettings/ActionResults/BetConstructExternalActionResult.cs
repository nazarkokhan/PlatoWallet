namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.BetConstruct;
using Microsoft.AspNetCore.Mvc;

public class BetConstructExternalActionResult : ActionResult
{
    public BetConstructExternalActionResult(IBetconstructResult result)
    {
        Result = result;
    }

    public IBetconstructResult Result { get; }
}