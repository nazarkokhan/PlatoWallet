namespace PlatipusWallet.Api.Results.External.ActionResults;

using Common.Result;
using Microsoft.AspNetCore.Mvc;

public class ExternalActionResult<TError> : ActionResult
{
    public ExternalActionResult(IBaseResult<TError> result)
    {
        Result = result;
    }

    public IBaseResult<TError> Result { get; }
}