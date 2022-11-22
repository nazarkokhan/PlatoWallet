namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;

public class PswExternalActionResult : ActionResult
{
    public PswExternalActionResult(IPswResult result)
    {
        Result = result;
    }

    public IPswResult Result { get; }
}