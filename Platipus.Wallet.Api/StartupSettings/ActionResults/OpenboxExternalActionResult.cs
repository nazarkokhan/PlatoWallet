namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Results.Openbox;

public class OpenboxExternalActionResult : ActionResult
{
    public OpenboxExternalActionResult(IOpenboxResult result)
    {
        Result = result;
    }

    public IOpenboxResult Result { get; }
}