namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;

public class OpenboxExternalActionResult : ActionResult
{
    public OpenboxExternalActionResult(IOpenboxResult result)
    {
        Result = result;
    }

    public IOpenboxResult Result { get; }
}