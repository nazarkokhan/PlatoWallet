namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Services.SweepiumGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/sweepium")]
[JsonSettingsName(WalletProvider.Sweepium)]
public sealed class ExternalSweepiumController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalSweepiumController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("launch")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        SweepiumGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}