namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Nemesis;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/nemesis")]
[JsonSettingsName(WalletProvider.Nemesis)]
public class ExternalNemesisController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalNemesisController(IMediator mediator) => _mediator = mediator;

    [HttpPost("launcher")]
    public async Task<IActionResult> Launcher(
        [FromBody] NemesisLauncherRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(
        [FromBody] NemesisCreateAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel(
        [FromBody] NemesisCancelAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("currencies")]
    public async Task<IActionResult> Currencies(
        [FromBody] NemesisCurrenciesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("round")]
    public async Task<IActionResult> Round([FromBody] NemesisRoundGameRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}