namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External;
using Application.Services.Hub88GamesApi.DTOs.Requests;
using Application.Services.Hub88GamesApi.DTOs.Responses;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/hub88")]
public class ExternalHub88Controller : ApiController
{
    private readonly IMediator _mediator;

    public ExternalHub88Controller(IMediator mediator)
        => _mediator = mediator;

    [HttpGet("gamelist")]
    [ProducesResponseType(typeof(List<Hub88GetGameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hub88Games(
        [FromQuery] GetHub88CasinoGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("round")]
    [ProducesResponseType(typeof(List<Hub88GetGameDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hub88Games(
        [FromQuery] ExternalHub88GetRoundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("prepaids")]
    [ProducesResponseType(typeof(List<Hub88PrepaidGamesApiResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hub88Games(
        [FromQuery] ExternalHub88PrepaidsListRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("reward")]
    [ProducesResponseType(typeof(Hub88GameApiCreateRewardResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hub88Games(
        ExternalHub88RewardsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("reward/cancel")]
    [ProducesResponseType(typeof(Hub88GameApiCreateRewardResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Hub88Games(
        ExternalHub88CancelRewardsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}