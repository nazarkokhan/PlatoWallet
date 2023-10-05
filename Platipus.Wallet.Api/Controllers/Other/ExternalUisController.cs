namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Uis;
using Application.Services.UisGamesApi.Dto;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/uis")]
[JsonSettingsName(WalletProvider.Uis)]
public sealed class ExternalUisController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalUisController(IMediator mediator) => _mediator = mediator;

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(UisAwardBonusRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("connect")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] UisGetLaunchUrlRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}