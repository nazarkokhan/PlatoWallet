namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Reevo;
using Application.Services.ReevoGamesApi.DTO;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/reevo")]
public class ExternalReevoController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalReevoController(IMediator mediator) => _mediator = mediator;

    [HttpPost("get-game")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGame(ReevoGetGameRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("add-free-rounds")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddFreeRounds(ReevoAddFreeRoundRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("remove-free-rounds")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveFreeRounds(ReevoRemoveFreeRoundsRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("get-game-history")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameHistory(ReevoGetGameHistoryRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("get-game-list")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameList(ReevoGetGameListRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}