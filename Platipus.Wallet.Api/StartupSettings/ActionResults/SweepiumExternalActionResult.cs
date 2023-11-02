using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Results.Sweepium;

namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

public class SweepiumExternalActionResult : ActionResult
{
    public SweepiumExternalActionResult(ISweepiumResult result)
    {
        Result = result;
    }

    public ISweepiumResult Result { get; }
}