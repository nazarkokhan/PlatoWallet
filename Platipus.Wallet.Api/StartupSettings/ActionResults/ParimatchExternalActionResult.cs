namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.PariMatch;
using Microsoft.AspNetCore.Mvc;

public class ParimatchExternalActionResult : ActionResult
{
    public ParimatchExternalActionResult(IParimatchResult result)
    {
        Result = result;
    }

    public IParimatchResult Result { get; }
}