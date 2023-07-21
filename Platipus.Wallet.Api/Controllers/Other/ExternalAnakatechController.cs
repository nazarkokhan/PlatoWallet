namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Responses.Anakatech.Base;
using Application.Services.AnakatechGamesApi.External;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security;

[Route("external/anakatech/")]
[ServiceFilter(typeof(AnakatechSecurityFilter))]
[JsonSettingsName(nameof(CasinoProvider.Anakatech))]
[ProducesResponseType(typeof(AnakatechErrorResponse), StatusCodes.Status200OK)]
public sealed class ExternalAnakatechController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalAnakatechController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("launchGame")]
    [ProducesResponseType(typeof(Uri), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerBalance(
        [FromBody] AnakatechLaunchGameRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.Data is null)
            return Ok(new AnakatechErrorResponse(false, 0, "Unknown error"));

        var gameUrl = response.Data.AbsoluteUri;
        return Redirect(gameUrl);
    }
}