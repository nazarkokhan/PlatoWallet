namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;

public class SoftswissExternalActionResult : ActionResult
{
    public SoftswissExternalActionResult(ISoftswissResult result)
    {
        Result = result;
    }

    public ISoftswissResult Result { get; }
}