namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Hub88;
using Application.Services.Hub88GamesApi.DTOs.Responses;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/hub88")]
[JsonSettingsName(WalletProvider.Hub88)]
public class ExternalHub88Controller : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalHub88Controller(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("game/url")]
    [ProducesResponseType(typeof(List<Hub88GameGameApiResponseItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] Hub88GetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rewards/create")]
    [ProducesResponseType(typeof(Hub88CreateAwardGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAward(
        [FromBody] Hub88CreateAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rewards/cancel")]
    [ProducesResponseType(typeof(Hub88DeleteAwardGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAward(
        [FromBody] Hub88DeleteAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("prepaids/list")]
    [ProducesResponseType(typeof(List<Hub88PrepaidGameApiResponseItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPrepaidsList(
        [FromBody] Hub88GetPrepaidsListRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/list")]
    public async Task<IActionResult> GetGameList(
        [FromBody] Hub88GetGameListRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/round")]
    public async Task<IActionResult> GetRound(
        [FromBody] Hub88GetRoundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}