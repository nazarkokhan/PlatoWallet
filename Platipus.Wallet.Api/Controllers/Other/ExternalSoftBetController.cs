namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Services.SoftBetGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("wallet/isoftbet")]
[JsonSettingsName(WalletProvider.SoftBet)]
public sealed class ExternalSoftBetController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalSoftBetController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("launch")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] SoftBetGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}