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

    [HttpPost("add-free-round")]
    [ProducesResponseType(typeof(ReevoAddFreeRoundsGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddFreeRound(ReevoAddFreeRoundRequest request, CancellationToken cancellationToken)
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