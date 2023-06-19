namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.Base;
using Microsoft.AspNetCore.Mvc;

public sealed class BaseExternalActionResult : ActionResult
{
    public BaseExternalActionResult(IBaseResult result)
    {
        Result = result;
    }

    public IBaseResult Result { get; }
}