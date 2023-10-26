namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Responses.Microgame.Base;
using Application.Services.MicrogameGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/microgame")]
[JsonSettingsName(WalletProvider.Microgame)]
[ProducesResponseType(typeof(MicrogameErrorResponse), StatusCodes.Status400BadRequest)]
public sealed class ExternalMicrogameController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalMicrogameController(IMediator mediator) => _mediator = mediator;

    [HttpPost("game/launch")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Launch(
        [FromBody] MicrogameLaunchApiRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}