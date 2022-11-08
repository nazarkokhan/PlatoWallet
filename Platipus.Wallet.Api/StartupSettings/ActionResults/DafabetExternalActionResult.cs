namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Results.Dafabet;

public class DafabetExternalActionResult : ActionResult
{
    public DafabetExternalActionResult(IDafabetResult result)
    {
        Result = result;
    }

    public IDafabetResult Result { get; }
}