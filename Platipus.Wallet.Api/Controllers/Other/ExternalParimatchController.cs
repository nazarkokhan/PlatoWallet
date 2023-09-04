namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Parimatch;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/parimatch")]
[JsonSettingsName(WalletProvider.Parimatch)]
public class ExternalParimatchController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalParimatchController(IMediator mediator) => _mediator = mediator;

    [HttpPost("launcher")]
    public async Task<IActionResult> Launcher(
        [FromBody] ParimatchLauncherRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(
        [FromBody] ParimatchCreateAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    public async Task<IActionResult> DeleteAward(
        [FromBody] ParimatchDeleteAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}