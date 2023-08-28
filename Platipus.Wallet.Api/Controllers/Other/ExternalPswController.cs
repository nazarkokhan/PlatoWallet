namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Psw;
using Application.Services.PswGameApi.Responses;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/psw")]
public class ExternalPswController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalPswController(IMediator mediator) => _mediator = mediator;

    [HttpPost("game/session")]
    [ProducesResponseType(typeof(PswGameSessionGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameSession(
        [FromBody] PswGameSessionRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/list")]
    [ProducesResponseType(typeof(PswGameListGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameList([FromBody] PswGameListRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("freebet/award")]
    [ProducesResponseType(typeof(PswFreebetAwardGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> FreebetAward(
        [FromBody] PswFreebetAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/buy")]
    [ProducesResponseType(typeof(PswGameBuyGameApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameBuy([FromBody] PswGameBuyRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}