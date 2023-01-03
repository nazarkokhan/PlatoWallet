namespace Platipus.Wallet.Api.StartupSettings.ActionResults;

using Application.Results.Everymatrix;
using Microsoft.AspNetCore.Mvc;

public class EveryMatrixExternalActionResult : ActionResult
    {
        public EveryMatrixExternalActionResult(IEverymatrixResult result)
        {
            Result = result;
        }

        public IEverymatrixResult Result { get; }
    }