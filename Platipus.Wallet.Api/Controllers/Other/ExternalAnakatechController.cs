namespace Platipus.Wallet.Api.Controllers.Other;

using System.Net;
using Abstract;
using Application.Responses.Anakatech.Base;
using Application.Services.AnakatechGamesApi.External;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/anakatech/")]
[JsonSettingsName(WalletProvider.Anakatech)]
[ProducesResponseType(typeof(AnakatechErrorResponse), StatusCodes.Status200OK)]
public sealed class ExternalAnakatechController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalAnakatechController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("launchGame")]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> LaunchGame(
        [FromBody] AnakatechLaunchGameRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        var launchGameScript = response.Data;
        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = (int)HttpStatusCode.OK,
            Content = launchGameScript
        };
    }
}