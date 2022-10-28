namespace Platipus.Wallet.Api.Results.External.ActionResults;

using Common.Result;
using Microsoft.AspNetCore.Mvc;

// public class ExternalActionResult<TError> : ActionResult
// {
//     public ExternalActionResult(IBaseResult<TError> result)
//     {
//         Result = result;
//     }
//
//     public IBaseResult<TError> Result { get; }
// }

public class ExternalActionResult : ActionResult
{
    public ExternalActionResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }
}

public class ExternalActionDatabetResult : ActionResult
{
    public ExternalActionDatabetResult(IDatabetResult result)
    {
        Result = result;
    }

    public IDatabetResult Result { get; }
}