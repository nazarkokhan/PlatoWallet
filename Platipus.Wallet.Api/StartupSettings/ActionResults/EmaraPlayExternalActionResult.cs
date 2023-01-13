namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.EmaraPlay;
using Microsoft.AspNetCore.Mvc;

public class EmaraPlayExternalActionResult : ActionResult
{
    public EmaraPlayExternalActionResult(IEmaraPlayResult result)
    {
        Result = result;
    }

    public IEmaraPlayResult Result { get; }
}