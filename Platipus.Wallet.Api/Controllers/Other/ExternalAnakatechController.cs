namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Responses.Anakatech.Base;
using Application.Services.AnakatechGamesApi.External;
using Domain.Entities.Enums;
using Extensions;
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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> LaunchGame(
        [FromBody] AnakatechLaunchGameRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}