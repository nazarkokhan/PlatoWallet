namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.Hub88;
using Microsoft.AspNetCore.Mvc;

public class Hub88ExternalActionResult : ActionResult
{
    public Hub88ExternalActionResult(IHub88Result result)
    {
        Result = result;
    }

    public IHub88Result Result { get; }
}