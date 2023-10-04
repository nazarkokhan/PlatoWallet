namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Services.BetconstructGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/betconstruct")]
[JsonSettingsName(WalletProvider.BetConstruct)]
public sealed class ExternalBetconstructController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalBetconstructController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("launch")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerInfo(
        BetconstructGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}