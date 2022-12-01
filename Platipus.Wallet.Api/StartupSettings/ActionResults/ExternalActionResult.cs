namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.Base;
using Microsoft.AspNetCore.Mvc;

public class ExternalActionResult : ActionResult
{
    public ExternalActionResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }
}

public class BaseExternalActionResult : ActionResult
{
    public BaseExternalActionResult(IBaseResult result)
    {
        Result = result;
    }

    public IBaseResult Result { get; }
}