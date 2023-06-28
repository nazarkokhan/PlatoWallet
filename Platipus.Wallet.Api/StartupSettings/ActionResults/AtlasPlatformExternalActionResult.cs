using Microsoft.AspNetCore.Mvc;

namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.Atlas;

public sealed class AtlasPlatformExternalActionResult : ActionResult
{
    public AtlasPlatformExternalActionResult(IAtlasResult result) => 
        Result = result;

    public IAtlasResult Result { get; }
}