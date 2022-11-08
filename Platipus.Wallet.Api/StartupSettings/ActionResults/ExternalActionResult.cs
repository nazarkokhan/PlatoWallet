namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;

public class ExternalActionResult : ActionResult
{
    public ExternalActionResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }
}