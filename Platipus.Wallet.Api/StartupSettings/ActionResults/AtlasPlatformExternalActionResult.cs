using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;

namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

public sealed class AtlasPlatformExternalActionResult : ActionResult
{
    public AtlasPlatformExternalActionResult(IAtlasPlatformResult result) => 
        Result = result;

    public IAtlasPlatformResult Result { get; }
}